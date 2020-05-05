using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerXO
{
    public class Cell
    {
        private IDictionary<string, IList<int>> neighbors;


        public byte XO { get; set; }


        public int Position { get; set; }
        public int Field_X { get; }
        public int Field_Y { get; }
        public int MaxNeighbors { get; }


        public Cell(byte xo, int position, int field_x, int field_y, int maxNeighbors)
        {
            neighbors = new Dictionary<string, IList<int>>();
            XO = xo;
            Position = position;
            Field_X = field_x;
            Field_Y = field_y;
            MaxNeighbors = maxNeighbors;

            IdentifyNeighbors();
        }


        private void IdentifyNeighbors()
        {
            int distanceToUpperBorder = Position / Field_X;
            int distanceToRightBorder = Field_X - Position % Field_X - 1;
            int distanceToLowerBorder = Field_Y - Position / Field_X - 1;
            int distanceToLeftBorder = Position % Field_X;

            int minDistanceToBorder, countNeighbors;

            #region Лево

            neighbors["Left - Right"] = new List<int>();

            countNeighbors = Math.Min(distanceToLeftBorder, MaxNeighbors);

            for (int i = countNeighbors; i > 0; --i)
                neighbors["Left - Right"].Add(Position - i);
            #endregion
            #region Право

            countNeighbors = distanceToRightBorder < MaxNeighbors ? distanceToRightBorder : MaxNeighbors;

            for (int i = 1; i < countNeighbors + 1; ++i)
                neighbors["Left - Right"].Add(Position + i);

            #endregion

            #region Лево-верх

            neighbors["LeftUp - RightDown"] = new List<int>();

            minDistanceToBorder = Math.Min(distanceToUpperBorder, distanceToLeftBorder); // отпределяем расстояние до ближайшей границы
            countNeighbors = Math.Min(minDistanceToBorder, MaxNeighbors); // определяем максимально количество соседей по направлению (регион)

            for (int i = countNeighbors; i > 0; --i) // добавляем индексы соседей
                neighbors["LeftUp - RightDown"].Add(Position - Field_X * i - i);

            #endregion
            #region Право-низ

            minDistanceToBorder = Math.Min(distanceToRightBorder, distanceToLowerBorder);
            countNeighbors = Math.Min(minDistanceToBorder, MaxNeighbors);

            for (int i = 1; i < countNeighbors + 1; ++i)
                neighbors["LeftUp - RightDown"].Add(Position + Field_X * i + i);

            #endregion

            #region Верх

            neighbors["Up - Down"] = new List<int>();

            countNeighbors = distanceToUpperBorder < MaxNeighbors ? distanceToUpperBorder : MaxNeighbors;

            for (int i = countNeighbors; i > 0; --i)
                neighbors["Up - Down"].Add(Position - Field_X * i);

            #endregion
            #region Низ

            countNeighbors = Math.Min(distanceToLowerBorder, MaxNeighbors);

            for (int i = 1; i < countNeighbors + 1; ++i)
                neighbors["Up - Down"].Add(Position + Field_X * i);

            #endregion

            #region Лево-низ

            neighbors["RightUp - LeftDown"] = new List<int>();

            minDistanceToBorder = Math.Min(distanceToLeftBorder, distanceToLowerBorder);
            countNeighbors = Math.Min(minDistanceToBorder, MaxNeighbors);

            for (int i = countNeighbors; i > 0; --i)
                neighbors["RightUp - LeftDown"].Add(Position + Field_X * i - i);

            #endregion
            #region Право-верх

            minDistanceToBorder = Math.Min(distanceToRightBorder, distanceToUpperBorder);
            countNeighbors = Math.Min(minDistanceToBorder, MaxNeighbors);

            for (int i = 1; i < countNeighbors + 1; ++i)
                neighbors["RightUp - LeftDown"].Add(Position - Field_X * i + i);

            #endregion
        }
        public int[] Win(List<Cell> collection)
        {
            List<int> countWinCells = new List<int>();

            foreach (KeyValuePair<string, IList<int>> route in neighbors)
            {
                foreach (int neighbor in route.Value)
                {
                    if (collection[neighbor].XO == XO) countWinCells.Add(collection[neighbor].Position);
                    else countWinCells.Clear();
                    if (countWinCells.Count == MaxNeighbors) return countWinCells.ToArray();
                }
            }
            return null;
        }
    }
}
