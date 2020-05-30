﻿using System;
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
        public StatusCell Status { get; set; }      // есть клетка или нет или она статичная
        public StatusCell NewStatus { get; set; }   // Новый статус клетки
        public bool active;                         // активная ячейка(true)- добавлена в список для обработки

        public CellLocation Location { get; set; }     // координаты ячейки на поле Field

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
            Copy(cell);
        }

        public void Copy(Cell cl)
        {
            Status = cl.Status;
            NewStatus = cl.NewStatus;
            active = cl.active;
            Location = cl.Location;
        }

        public bool IsLive() => (Status == StatusCell.Yes);     // клетка есть
        public bool IsStatic() => (Status == StatusCell.Static);// статичная клетка
        public bool IsNoCell() => (Status == StatusCell.No);    // клетки нет

        /// <summary>
        /// Анализ следующего шага.
        /// </summary>
        public void AnalysisNextStep(Field field)
        {
            int numberNearest = field.NumberLiveCells(Location.X, Location.Y);

            // Новое состояние клетки приравниваем к старому
            NewStatus = Status;

            if (numberNearest == 3)
            {
                if (!IsLive())          // клетки нет
                {
                    // клетка появится
                    NewStatus = StatusCell.Yes;
                }

                field.AddCellForNextStep(this);
            }
            else
            if (numberNearest == 2)
            {
                if (IsLive())
                {
                    // клетка остаётся на следующий ход
                    field.AddCellForNextStep(this);
                }
                else
                {
                    // делаем неактивной
                    active = false;
                }
            }
            else
            {
                if (IsLive())   // клетка есть
                {
                    // клетка исчезает
                    NewStatus = StatusCell.No;
                }

                // make inactive
                active = false;
            }
        }

        /// <summary>
        /// Меняем состояние клетки м зависимости от произведённого ранее анализа
        /// </summary>
        public void ChangeStatus(Field field)
        {
            if (IsChangeStatus())
            {
                if (NewStatus == StatusCell.Yes)    // клетка должна появиться
                {
                    field.AddCell(this);
                }

                Status = NewStatus;

                field.AddToDraw(this);
            }

            if (!active) // неактивные(погибшие) клетки удаляем с поля
            {
                field.RemoveCell(this);
            }
        }

        public bool IsChangeStatus() => (Status != NewStatus);

        public void Draw(Graphics g, BitmapCellsStorage b)
        {
            Bitmap bm = b.GetBitmap(Status);

            // Координаты точки для отрисовки
            int x = Location.X * bm.Width;
            int y = Location.Y * bm.Height;

            g.DrawImage(bm, x, y);
        }

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

        internal void Offset(CellLocation begin)
        {
            Location = new CellLocation(Location.X + begin.X, Location.Y + begin.Y);
        }
    }
}

