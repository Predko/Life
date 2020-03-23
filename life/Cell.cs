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

	public enum StatusCell { Yes, No };

	public class CellEvent 
	{
		public Calccmd cmd;
		public Graphics G { get; private set; }

		public CellEvent(Calccmd c, Graphics g = null)
		{
			cmd = c;
			G = g;
		}
	}


	public class Cell : IComparable<Cell>, IEquatable<Cell>, IDraw
	{
		public StatusCell Status { get; set; }		// есть клетка или нет
		public StatusCell NewStatus { get; set; }	// есть клетка или нет
		private readonly Field field;
		public FieldLocation Location { get; set; }		// координаты ячейки на поле Field

		public bool active;             // активная ячейка(true)- добавлена в список событий
		public bool isStaticCell;       // Статическая(неизменная) ячейка. В список событий не добавляется

		public Cell(Field fld, FieldLocation fl)
		{
			field = fld;
			Status = StatusCell.No;
			isStaticCell = false;

			SetCell();

			Location = fl;
		}

		public Cell(Field fld, int x, int y):this(fld, new FieldLocation(x,y))
		{
		}

		public void Copy(Cell cl)
		{
			SetCell(cl.Status);
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

		// Анализ следующего шага.
		public void AnalysisNextStep()
		{
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

		public void ChangeStatus()
		{
			if (Status != NewStatus)
			{
				if (NewStatus == StatusCell.Yes)
				{ 
					field.AddCell(this);
				}

				Status = NewStatus;
			}
			
			if (!active)
			{
				field.RemoveCell(this);
			}
		}

		public void SetStaticCell()
		{
			isStaticCell = true;
			SetCell(StatusCell.Yes);
		}

		// отрисовываем ячейку
		public virtual void Draw(Graphics g)
		{
			if (Status == StatusCell.Yes)
			{
				g.DrawImage(field.bitmapCellYesNo, GetRectangle(), field.rectCellYes, GraphicsUnit.Pixel);
			}
			else
			if (Status == StatusCell.No)
			{
				g.DrawImage(field.bitmapCellYesNo, GetRectangle(), field.rectCellNo, GraphicsUnit.Pixel);
			}
		}

		// Преобразование координат поля в координаты рабочей области формы
		private Rectangle GetRectangle() => new Rectangle()
											{
												X = field.rectangle.X + Location.X * field.CellSize.Width + 1,
												Y = field.rectangle.Y + Location.Y * field.CellSize.Height + 1,
												Width = field.CellSize.Width - 2,
												Height = field.CellSize.Height - 2
											};

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

