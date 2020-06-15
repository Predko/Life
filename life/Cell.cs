using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace life
{
    /// <summary>
    /// Клетка есть, нет, статичная 
    /// </summary>
    public enum StatusCell { Yes, No, Static };

    public class Cell : IComparable<Cell>, IEquatable<Cell>
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
        public Point Location { get; set; }

        public Cell(Point fl)
        {
            Status = StatusCell.No;
            NewStatus = StatusCell.No;

            Location = fl;
        }

        public Cell(int x, int y) : this(new Point(x, y))
        {
        }

        public Cell(Cell cell)
        {
            Copy(cell);
        }

        /// <summary>
        /// Копирует клетку
        /// </summary>
        /// <param name="cell"></param>
        public void Copy(Cell cell)
        {
            Status = cell.Status;
            NewStatus = cell.NewStatus;
            active = cell.active;
            Location = cell.Location;
        }

        /// <summary>
        /// Живая клетка.
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

        /// <summary>
        /// Отрисовывает клетку
        /// </summary>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void Draw(Graphics g, BitmapCellsStorage b) => Draw(g, b, Location);

        /// <summary>
        /// Отрисовка клетки в заданной точке.
        /// <param name="p">Положение клетки.</param>
        /// </summary>
        public void Draw(Graphics g, BitmapCellsStorage b, Point p)
        {
            Bitmap bm = b.GetBitmap(Status);

            // Координаты точки для отрисовки
            
            p.X *= bm.Width;
            p.Y *= bm.Height;

            g.DrawImage(bm, p);
        }

        /// <summary>
        /// Смещает данную клетку на смещение указанное координатами.
        /// </summary>
        /// <param name="begin">Смещение.</param>
        public void Offset(Point begin)
        {
            begin.Offset(Location);

            Location = begin;
        }

        /// <summary>
        /// Определяет, имеет ли данная клетка те же координаты.
        /// </summary>
        /// <param name="other">Проверяемая клетка.</param>
        /// <returns></returns>
        public int CompareTo(Cell other)
        {
            int res = Location.Y.CompareTo(other.Location.Y);

            if (res == 0)
            {
                return Location.X.CompareTo(other.Location.X);
            }

            return res;
        }
        
        /// <summary>
        /// Рассчитывает хэшкод, зависящий от координат X и Y.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => ((Location.Y & 0x7FFF) << 15) | (Location.X & 0x7FFF);

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

