using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace life
{
    // отрисовка клетки
    interface IDraw
    {
        void Draw(Graphics g, BitmapCellsStorage b);
    }

    /// <summary>
    /// Клетка есть, нет, статичная 
    /// </summary>
    public enum StatusCell { Yes = 0, Static = 1, No = 2 };

    public class Cell : IComparable<Cell>, IEquatable<Cell>, IDraw
    {
        /// <summary>
        /// Состояние клетки.
        /// </summary>
        public StatusCell Status { get; set; }

        /// <summary>
        /// Новый статус клетки.
        /// </summary>
        public StatusCell NewStatus { get; set; }

        /// <summary>
        /// Активная клетка(true)- добавлена в список для обработки.
        /// </summary>
        public bool active;

        /// <summary>
        /// Координаты клетки на поле Field.
        /// </summary>
        public CellLocation Location { get; set; }

        public Cell(CellLocation fl)
        {
            Status = StatusCell.No;
            NewStatus = StatusCell.No;

            Location = fl;
        }

        public Cell(int x, int y) : this(new CellLocation(x, y))
        {
        }

        public Cell(Cell cell)
        {
            Set(cell);
        }

        public void Set(Cell cell)
        {
            Status = cell.Status;
            NewStatus = cell.NewStatus;
            active = cell.active;
            Location = cell.Location;
        }

        /// <summary>
        /// Нормальная клетка.
        /// </summary>
        public bool IsLive => (Status == StatusCell.Yes);

        /// <summary>
        /// Статичная клетка.
        /// </summary>
        public bool IsStatic => (Status == StatusCell.Static);

        /// <summary>
        /// Клетки нет. 
        /// </summary>
        public bool IsNoCell => (Status == StatusCell.No);
        
        /// <summary>
        /// Активная клетка. Эта клетка учавствует в рассчёте следующего хода.
        /// </summary>
        public bool IsActive => active;

        /// <summary>
        /// Изменится ли статус клетки на следующем ходу.
        /// </summary>
        public bool IsChangeStatus => (Status != NewStatus);

        public void Draw(Graphics g, BitmapCellsStorage b)
        {
            Bitmap bm = b.GetBitmap(Status);

            // Координаты точки для отрисовки
            int x = Location.X * bm.Width;
            int y = Location.Y * bm.Height;

            g.DrawImage(bm, x, y);
        }

        /// <summary>
        /// Смещает данную клетку на смещение указанное координатами.
        /// </summary>
        /// <param name="begin">Смещение.</param>
        public void Offset(CellLocation begin)
        {
            Location = new CellLocation(Location.X + begin.X, Location.Y + begin.Y);
        }

        /// <summary>
        /// Определяет, имеет ли данная клетка те же координаты.
        /// </summary>
        /// <param name="other">Проверяемая клетка.</param>
        /// <returns></returns>
        public int CompareTo(Cell other) => Location.CompareTo(other.Location);

        public override int GetHashCode() => Location.GetHashCode();

        public bool Equals(Cell other)
        {
            return Location.Equals(other.Location) &&
                   Status == other.Status &&
                   NewStatus == other.NewStatus &&
                   active == other.active;
        }

        public override bool Equals(object obj) => obj is Cell other && Equals(other);
    }
}

