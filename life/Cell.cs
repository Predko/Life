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


	public class Cell : IDraw
	{
		public StatusCell Status { get; private set; }	// символ L  или E - есть клетка или нет
		private readonly Field field;
		public FieldLocation Location { get; set; }		// координаты ячейки на поле Field

		public bool ischange;			// изменение клетки: true - должно измениться, false - нет
		public bool active;             // активная ячейка(true)- добавлена в список событий
		public bool isStaticCell;		// Статическая(неизменная) ячейка. В список событий не добавляется



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
			ischange = false;
			active = false;
			if (Status != cl)
			{
				field.listCellLocationToDraw.Add(this.Location);
				Status = cl;
			}
		}

		public void SetCellNo() => SetCell(StatusCell.No);

		public void SetCellYes() => SetCell(StatusCell.Yes);

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
				g.FillRectangle(field.brushCellYes, GetRectangle());
			}
			else
			if (Status == StatusCell.No)
			{
				g.FillRectangle(field.brushCellNo, GetRectangle());
			}
		}

		// Преобразование координат поля в координаты рабочей области формы
		private Rectangle GetRectangle() => new Rectangle()
											{
												X = Location.X * field.CellSize.Width + 1,
												Y = Location.Y * field.CellSize.Height + 1,
												Width = field.CellSize.Width - 2,
												Height = field.CellSize.Height - 2
											};

		// рассчитываем следующий ход ячейки(жива/нет) или меняем статус клетки если cng == true,
		// удаляется из списка событий, если нет(false),
		// удаляется из списка событий, если рядом нет живых ячеек первого поколения,
		// если рядом больше двух живых ячеек, отмечает себя живой, проверяем соседние
		public void Calc(CellEvent ce)
		{
			switch (ce.cmd)
			{
				case Calccmd.apply_changes:

					ApplyChanges();

					break;

				case Calccmd.delete_not_active:

					DeleteNotActive();
					break;

				case Calccmd.analysis_nextstep:

					AnalysisNextStep();

					break;

				case Calccmd.del_calc_func:

					field.ListCells -= Calc;
					break;

				case Calccmd.draw:

					if (ce.G != null)
					{
						Draw(ce.G);
					}
					break;
			}
		}

		// Применить изменения следующего шага
		private void ApplyChanges()
		{
			if (ischange)
			{
				if (Status == StatusCell.No)
				{
					Status = StatusCell.Yes;                            // клетка добавляется
				}
				else
				{
					Status = StatusCell.No;                             // клетка удаляется
				}

				field.listCellLocationToDraw.Add(this.Location);

				ischange = false;
			}

		}

		// Анализ следующего шага
		private void AnalysisNextStep()
		{
			int numberNearest = field.NumberLiveCells(Location.X, Location.Y, IsLive());

			if (numberNearest == 3)
			{
				if (Status == StatusCell.No)        // клетки нет
					ischange = true;                // клетка рождается
			}
			else if (numberNearest != 2)
			{
				if (Status == StatusCell.Yes)       // клетка есть
				{
					ischange = true;                // клетка исчезает 
				}
			}
		}

		// Удаляем неактивные клетки и добавляем близлежащие к живым
		private void DeleteNotActive()
		{

			int numberNearest = field.NumberLiveCells(Location.X, Location.Y, IsLive());// добавляем окружающие в список события

			if (numberNearest == 0 && Status == StatusCell.No)
			{                                   // клетки нет и вокруг нет живых - удаляем из списка события
				field.ListCells -= Calc;
				active = false;
				field.listCellLocationToDraw.Add(this.Location);
			}
		}

	}
}

