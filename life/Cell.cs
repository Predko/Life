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


	public enum Calccmd { apply_changes, analysis_nextstep, del_calc_func, delete_not_active, draw };
	public enum Cell_is { yes, no };

	public enum StatusCell { Yes, No, Nil };

	public class CellEvent 
	{
		public Calccmd cmd;

		public CellEvent(Calccmd c)
		{
			cmd = c;
		}
	}


	public class Cell : IComparable<Cell>, IEquatable<Cell>, IDraw
	{
		public StatusCell Status { get; set; }		// есть клетка или нет
		public StatusCell NewStatus { get; set; }	// есть клетка или нет
		public Field field;
		public FieldLocation Location { get; set; }     // координаты ячейки на поле Field

		private Bitmap bitmap;	// изображение клетки

		public bool active;             // активная ячейка(true)- добавлена в список событий
		public bool isStaticCell;       // Статическая(неизменная) ячейка. В список событий не добавляется

		public Cell(Field fld, FieldLocation fl)
		{
			field = fld;
			Status = StatusCell.No;
			NewStatus = StatusCell.Nil;
			isStaticCell = false;

			SetCell();

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
			isStaticCell = cl.isStaticCell;
			Location = cl.Location;
		}

		public bool IsLive() => Status == StatusCell.Yes; // клетка есть

		public void SetCell(StatusCell cl = StatusCell.No)
		{
			if (Status != cl)
			{
				Status = cl;
			}
		}

		public void SetCellNo() => SetCell(StatusCell.No);

		public void SetCellYes() => SetCell(StatusCell.Yes);

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

		public void SetStaticCell()
		{
			isStaticCell = true;
			SetCell(StatusCell.Yes);
		}

		// отрисовываем ячейку на битовой карте
		public virtual void Draw(Graphics g)
		{
			// Координаты точки для отрисовки
			int x = Location.X * field.CellSize;
			int y = Location.Y * field.CellSize;

			Bitmap bm = (isStaticCell) ? field.CellStaticBitmap : field.CellBitmap;

			if (Status == StatusCell.Yes)
			{
				g.DrawImage(bm, x, y, field.rectCellYes, GraphicsUnit.Pixel);
			}
			else
			if (Status == StatusCell.No)
			{
				g.DrawImage(bm, x, y, field.rectCellNo, GraphicsUnit.Pixel);
			}
		}

		public int CompareTo(Cell other) => Location.CompareTo(other.Location);

		public override int GetHashCode() => Location.GetHashCode();

		public bool Equals(Cell other)
		{
			return Location.Equals(other.Location) &&
				   Status == other.Status &&
				   NewStatus == other.NewStatus &&
				   active == other.active &&
				   isStaticCell == other.isStaticCell;
		}

		public override bool Equals(object obj) => obj is Cell other && Equals(other);
	}
}

