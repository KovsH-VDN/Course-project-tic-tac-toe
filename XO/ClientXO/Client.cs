using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientXO
{
    public class Client : BindableBase
    {
        public ObservableCollection<Cell> Cells { get; set; }
        private Action<string> showMessage;
        private NetworkStream stream;
        private IList<string> statusMessage = new List<string>()
        {
            "Ожидание второго игрока", // 0
            "Ход противника", // 1
            "Ваш ход", // 2
            "Победил крестик", // 3
            "Победил нолик", // 4
            "Ничья", // 5
            "Максимальное количество игроков", // 6
            "Игрок с таким ником уже есть" // 7
        };

        private string status = "";
        public string Status
        {
            get => status;
            set
            {
                if (status.Equals(value)) return;
                status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }

        private bool step;
        public bool Step
        {
            get => step;
            set
            {
                if (step.Equals(value)) return;
                step = value;
                RaisePropertyChanged(nameof(Step));
            }
        }


        private bool isEnabledChat;
        public bool IsEnabledChat
        {
            get => isEnabledChat;
            set
            {
                if (isEnabledChat.Equals(value)) return;
                isEnabledChat = value;
                RaisePropertyChanged(nameof(IsEnabledChat));
            }
        }

        public TcpClient Server { get; set; }
        public string ServerIP { get; set; }
        public int MyPort { get; set; }

        private string login = "";
        public string Login
        {
            get => login;
            set
            {
                if (login.Equals(value)) return;
                login = value;
                RaisePropertyChanged(nameof(Login));
            }
        }

        private string enemyLogin = "";
        public string EnemyLogin
        {
            get => enemyLogin;
            set
            {
                if (enemyLogin.Equals(value)) return;
                enemyLogin = value;
                RaisePropertyChanged(nameof(EnemyLogin));
            }
        }

        public int Field_X => 25;
        public int Field_Y => 25;
        public int Neighbors => 4;


        public Client(string serverIP, int port, string login, Action<string> message)
        {
            Server = new TcpClient();
            ServerIP = serverIP;
            MyPort = port;
            Login = login;
            showMessage = message;

            Cells = new ObservableCollection<Cell>();

            for (int i = 0; i < Field_X * Field_Y; ++i)
                Cells.Add(new Cell(0));

            Connect();
        }
        
        public async void Connect()
        {
            try
            {
                await Server.ConnectAsync(IPAddress.Parse(ServerIP), MyPort);
                stream = Server.GetStream();
                
                BinaryWriter writer = new BinaryWriter(stream);
                BinaryReader reader = new BinaryReader(stream);
                await Task.Run(() => writer.Write(Login));

                byte statusCode = await Task.Run(() => reader.ReadByte());
                Status = statusMessage[statusCode];
                

                if (statusCode == 6 || statusCode == 7) return;

                if (statusCode == 0)
                {
                    Step = false;
                    IsEnabledChat = false;
                }

                EnemyLogin = await Task.Run(() => reader.ReadString());

                if (statusCode == 1)
                {
                    string name = Login;
                    Login = EnemyLogin;
                    EnemyLogin = name;
                }

                Status = statusMessage[await Task.Run(() => reader.ReadByte())];

                IsEnabledChat = true;
                Step = true;
                Listen();
            }
            catch (Exception ex) when(ex is SocketException || ex is IOException)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void Listen()
        {
            try
            {
                while (true)
                    await ReceivingCommand();
            }
            catch (Exception ex)
            {
                Server.Close();
                stream.Close(0);
            }
        }
        private async Task ReceivingCommand()
        {
            try
            {
                BinaryReader reader = new BinaryReader(stream);
                byte command = await Task.Run(() => reader.ReadByte()); // принимаем команду

                switch(command)
                {
                    case 1:
                        showMessage(await Task.Run(() => reader.ReadString()));
                        break;
                    case 2:
                        await AcceptGameData();
                        break;
                }
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException)
            {
                MessageBox.Show(ex.Message);
                throw new Exception();
            }
        }
        private async Task AcceptGameData()
        {
            BinaryReader reader = new BinaryReader(stream);

            int index = await Task.Run(() => reader.ReadInt32()); // принимаем ячейку
            byte indexValue = await Task.Run(() => reader.ReadByte()); // значение ячейки

            Cells[index].XO = indexValue;

            Step = await Task.Run(() => reader.ReadBoolean()); // ожидание/ход
            Cells[index].IsWin = await Task.Run(() => reader.ReadBoolean()); // данные о победе
            Status = statusMessage[await Task.Run(() => reader.ReadByte())]; // код статус-сообщения

            if (Cells[index].IsWin)
                for (int i = 0; i < Neighbors; ++i)
                    Cells[await Task.Run(() => reader.ReadInt32())].IsWin = true;
        }

        public async Task SentMessage(byte commandCode, object data)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            await Task.Run(() => writer.Write(commandCode));
            await Task.Run(() => writer.Write((string)data));
        }
        public async Task SentGameData(byte commandCode, object data)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            await Task.Run(() => writer.Write(commandCode));
            await Task.Run(() => writer.Write((int)data));
        }
        
    }
}
