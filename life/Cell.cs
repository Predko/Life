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


	public enum Calccmd { set_status, calc_cell, del_calc_func, draw };
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


	public class Cell : IDraw
	{
		public StatusCell Status { get; private set; }	// символ L  или E - есть клетка или нет
		private readonly Field field;
		public FieldLocation Location { get; set; }		// координаты ячейки на поле Field

		public bool ischange;			// изменение клетки: true - должно измениться, false - нет
		public bool active;             // активная ячейка(true)- добавлена в список событий
		public bool isNeedRedraw;		// требуется перерисовка


		public bool IsLive()
		{
			return Status == StatusCell.Yes; // клетка первого поколения(не только что созданная) и клетка живая
		}


		public Cell(Field fld, FieldLocation fl)
		{
			field = fld;
			Status = StatusCell.No;

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

		public void SetCell(StatusCell cl = StatusCell.No)
		{
			ischange = false;
			active = false;
			if (Status != cl)
			{
				isNeedRedraw = true;
				field.listCellToDraw.Add(this);
				Status = cl;
			}
		}

		public void SetCellNo()
		{
			SetCell(StatusCell.No);
		}

		public void SetCellYes()
		{
			SetCell(StatusCell.Yes);
		}

		// отрисовываем ячейку
		public virtual void Draw(Graphics g)
		{
			if (Status == StatusCell.Yes)
			{
				g.FillEllipse(field.brushCellYes, GetRectangle());
			}
			else
			if (Status == StatusCell.No)
			{
				g.FillEllipse(field.brushCellNo, GetRectangle());
			}
		}

		// Преобразование координат поля в координаты рабочей области формы
		private Rectangle GetRectangle() => new Rectangle()
											{
												X = Location.X * field.CellSize.Width,
												Y = Location.Y * field.CellSize.Height,
												Width = field.CellSize.Width,
												Height = field.CellSize.Height
											};

		// рассчитываем следующий ход ячейки(жива/нет) или меняем статус клетки если cng == true,
		// удаляется из списка событий, если нет(false),
		// удаляется из списка событий, если рядом нет живых ячеек первого поколения,
		// если рядом больше двух живых ячеек первого поколения, отмечает себя живой, проверяем соседние
		public void Calc(CellEvent ce)
		{
			switch (ce.cmd)
			{
				case Calccmd.set_status:

					CellSetStatus();

					break;

				case Calccmd.del_calc_func:

					field.ListCells -= Calc;
					break;

				case Calccmd.calc_cell:

					CellCalcNextStep();

					break;

				case Calccmd.draw:

					if (ce.G != null)
					{
						Draw(ce.G);
					}
					break;
			}
		}

		private void CellSetStatus()
		{
			if (ischange)
			{
				if (Status == StatusCell.No)
				{
					Status = StatusCell.Yes;                            // клетка появится
					field.IsLiveCount(Location.X, Location.Y, IsLive());// добавляем окружающие в список события
				}
				else
				{
					Status = StatusCell.No;                             // клетка исчезнет
				}

				field.listCellToDraw.Add(this);

				ischange = false;
			}
		}

		private void CellCalcNextStep()
		{
			int countcells = field.IsLiveCount(Location.X, Location.Y, IsLive());

			if (countcells == 3)
			{
				if (Status == StatusCell.No)        // клетки нет
					ischange = true;                // клетка рождается
			}
			else if (countcells != 2)
			{

				if (Status == StatusCell.Yes)       // клетка есть
				{
					ischange = true;                // клетка исчезает 
				}
				else
				if (countcells == 0)
				{                                   // клетки нет и вокруг нет живых - удаляем из списка события
					field.ListCells -= Calc;
					active = false;
				}
			}
		}
	}
}
