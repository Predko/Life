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
		void Draw(Graphics g);
	}

	/// <summary>
	/// Клетка есть, нет, статичная 
	/// </summary>
	public enum StatusCell { Yes, No, Static };

	public class Cell : IComparable<Cell>, IEquatable<Cell>, IDraw
	{
		public StatusCell Status { get; set; }		// есть клетка или нет или она статичная
		public StatusCell NewStatus { get; set; }	// Новый статус клетки
		public bool active;							// активная ячейка(true)- добавлена в список для обработки

		public Field field;
		public FieldLocation Location { get; set; }     // координаты ячейки на поле Field

		public Cell(Field fld, FieldLocation fl)
		{
			field = fld;
			Status = StatusCell.No;
			NewStatus = StatusCell.No;

			Location = fl;
		}

		public Cell(Field fld, int x, int y):this(fld, new FieldLocation(x,y))
		{
		}

		public Cell(Cell cell)
		{
			Copy(cell);
		}

		public void Copy(Cell cl)
		{
			field = cl.field;
			Status = cl.Status;
			NewStatus = cl.NewStatus;
			active = cl.active;
			Location = cl.Location;
		}

		public bool IsLive() => (Status == StatusCell.Yes);		// клетка есть
		public bool IsStatic() => (Status == StatusCell.Static);// статичная клетка
		public bool IsNoCell() => (Status == StatusCell.No);    // клетки нет

		/// <summary>
		/// Анализ следующего шага.
		/// </summary>
		public void AnalysisNextStep()
		{
			//if (isStaticCell)
			//{
			//	field.NewListCells.Add(this);
			//}
			
			int numberNearest = field.NumberLiveCells(Location.X, Location.Y);

			NewStatus = Status; // Новое состояние клетки приравниваем к старому

			if (numberNearest == 3)
			{
				if (!IsLive())			// клетки нет
				{
					NewStatus = StatusCell.Yes;            // клетка родится
				}

				field.NewListCells.Add(this);
			}
			else 
			if (numberNearest == 2)
			{
				if (IsLive())
				{
					field.NewListCells.Add(this);       // клетка остаётся на следующий ход
				}
				else
				{
					active = false;		// делаем неактивной
				}
			}
			else 
			{
				if (IsLive())   // клетка есть
				{
					NewStatus = StatusCell.No;         // клетка исчезает
				}

				active = false; // make inactive
			}
		}

		/// <summary>
		/// Меняем состояние клетки м зависимости от произведённого ранее анализа
		/// </summary>
		public void ChangeStatus()
		{
			if (IsChangeStatus())
			{
				if (NewStatus == StatusCell.Yes)	// клетка должна появиться
				{ 
					field.AddCell(this);
				}

				Status = NewStatus;
				
				field.ListCellsForDraw.Add(this);
			}
			
			if (!active) // неактивные(погибшие) клетки удаляем с поля
			{
				field.RemoveCell(this);
			}
		}

		public bool IsChangeStatus() => (Status != NewStatus);

		// отрисовываем ячейку на битовой карте
		public virtual void Draw(Graphics g)
		{
			// Координаты точки для отрисовки
			int x = Location.X * field.CellSize;
			int y = Location.Y * field.CellSize;

			if (IsLive())
			{
				g.DrawImage(field.CellBitmap, x, y, field.rectCellYes, GraphicsUnit.Pixel);
			}
			else
			if (IsNoCell())
			{
				g.DrawImage(field.CellBitmap, x, y, field.rectCellNo, GraphicsUnit.Pixel);
			}
			else
			if(IsStatic())
			{
				g.DrawImage(field.CellStaticBitmap, x, y, field.rectCellYes, GraphicsUnit.Pixel);
			}
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
	}
}

