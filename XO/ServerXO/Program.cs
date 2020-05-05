using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServerXO
{
    class Program
    {
        private int port = 7018;
        private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private TcpListener server;
        public IPAddress MyAddreses => Dns.GetHostAddresses(Dns.GetHostName()).Where(addres => addres.AddressFamily == AddressFamily.InterNetwork).First();

        private List<Cell> Cells = new List<Cell>();
        private bool step = true;
        public static int Field_X => 25;
        public static int Field_Y => 25;
        public static int Neighbors => 4;
        private int standoff = Field_X * Field_Y;

        static void Main(string[] args) => new Program().Run().Wait();

        private async Task Run()
        {
            for (int i = 0; i < Field_X * Field_Y; ++i)
                Cells.Add(new Cell(0, i, Field_X, Field_Y, Neighbors));

            server = new TcpListener(MyAddreses, port);
            server.Start(10);
            Console.WriteLine($"Сервер запущен. Рабочий порт - {port}, IP адрес - {MyAddreses}");

            await AcceptConnection();
        }

        private async Task AcceptConnection()
        {
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();

                NetworkStream stream = client.GetStream();

                BinaryReader reader = new BinaryReader(stream);
                BinaryWriter writer = new BinaryWriter(stream);

                string login = await Task.Run(() => reader.ReadString());

                if (clients.ContainsKey(login))
                {
                    Console.WriteLine("Соединение отклонено. Игрок с таким ником уже есть.");
                    await Task.Run(() => writer.Write((byte)7));
                    client.Close();
                    continue;
                }

                if (clients.Count == 2)
                {
                    Console.WriteLine("Соединение отклонено. Максимально количество игроков.");
                    await Task.Run(() => writer.Write((byte)6));
                    client.Close();
                    continue;
                }

                lock (clients)
                    clients[login] = client;

                await Task.Run(() => writer.Write((byte)(clients.Count - 1)));

                Console.WriteLine($"Клиент {login} подключился.");
                
                if (clients.Count == 2)
                {
                    KeyValuePair<string, TcpClient> firstGamer = clients
                                                                    .Where(gamer => gamer.Key != login)
                                                                    .First();
                    await Task.Run(() => writer.Write(firstGamer.Key));
                    await Task.Run(() => writer.Write((byte)1));

                    writer = new BinaryWriter(firstGamer.Value.GetStream());
                    await Task.Run(() => writer.Write(login));
                    await Task.Run(() => writer.Write((byte)2));
                }

                ListenClient(login);
            }
        }

        private async void ListenClient(string login)
        {

            NetworkStream stream = clients[login].GetStream();
            try
            {
                while (true)
                    await ReceivingCommand(stream, login);
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException)
            {
                Console.WriteLine($"Клиент {login} разорвал соединение.");
                clients[login].Close();
                clients.Remove(login);
            }
        }

        private async Task ReceivingCommand(NetworkStream incomingStream, string login)
        {
                BinaryReader reader = new BinaryReader(incomingStream);
                byte command = await Task.Run(() => reader.ReadByte());

                switch (command)
                {
                    case 1:
                    {
                        Console.WriteLine($"{login} отправил сообщение в чат:");
                        await SentMessageToChat(incomingStream, login, command);
                    }
                    break;
                    case 2:
                    {
                        Console.WriteLine($"{login} совершил ход.");

                        await AnalysisGame(reader, login, command);
                    }
                    break;
            }
        }
        private async Task SentMessageToChat(NetworkStream incomingStream, string login, byte command) 
        {
            BinaryReader reader = new BinaryReader(incomingStream);
            string message = $"{login}: " + await Task.Run(()=> reader.ReadString());
            Console.WriteLine(message);

            BinaryWriter writer;
            foreach (TcpClient toWhom in clients.Values)
            {
                writer = new BinaryWriter(toWhom.GetStream());
                writer.Write(command);
                writer.Write(message);
            }
        }
        private async Task AnalysisGame(BinaryReader reader, string login, byte command)
        {
            int indexCell = await Task.Run(() => reader.ReadInt32());
            Console.Write($"Ячейка - {indexCell}. ");
            Cells[indexCell].XO = (byte)(step ? 1 : 2);
            BinaryWriter writer;
            int [] countWinCells = await Task.Run(() => Cells[indexCell].Win(Cells));
            bool win = countWinCells != null && countWinCells.Length == Neighbors;
            lock(server)
                --standoff;
            Console.WriteLine($"Ходов осталось - {standoff}.");
            foreach (TcpClient client in clients.Values)
            {
                writer = new BinaryWriter(client.GetStream());


                await Task.Run(() => writer.Write(command)); // отправляем команду - ход
                await Task.Run(() => writer.Write(indexCell)); // какая ячейка
                await Task.Run(() => writer.Write(Cells[indexCell].XO)); // значение ячейки
                await Task.Run(() => writer.Write(win ? false : !step)); // кто ходит
                await Task.Run(() => writer.Write(win)); // информация о победной ячейке
                if (win)
                {
                    if (step) await Task.Run(() => writer.Write((byte)3));
                    else await Task.Run(() => writer.Write((byte)4));

                    foreach(int cell in countWinCells)
                        await Task.Run(() => writer.Write(cell));
                }
                else
                {
                    if (standoff == 0) await Task.Run(() => writer.Write((byte)5));
                    else if (!step) await Task.Run(() => writer.Write((byte)2));
                    else await Task.Run(() => writer.Write((byte)1));
                    step = !step;
                }
            }
            lock (server)
            {
                step = !step;
            }
        }
        
    }
}
