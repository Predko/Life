using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace life
{
	public struct FieldLocation
	{
		public int X;
		public int Y;

		public FieldLocation(int x, int y)
		{
			X = x;
			Y = y;
		}

		//public static implicit operator FieldLocation(Point p)
		//{
		//	return new FieldLocation() { X = p.X, Y = p.Y };
		//}

	}

	public delegate void CalcCell(CellEvent ce);

	// поле, верхняя часть соединена с нижней, левая с правой
	public class Field
	{
		private readonly int length;
		private readonly int width;

		public Size CellSize { get; set; }

		public Brush brushCellYes;
		public Brush brushCellNo; 

		// массив координат ячеек вокруг данной
		private readonly FieldLocation[] Dxy = new FieldLocation[8];

		public CellArray fld;


		public event CalcCell ListCells;


		// Длина и ширина в ячейках игрового поля
		public Field(int lenght, int width)
		{
			this.length = lenght;
			this.width = width;
			fld = new CellArray(this, length, width);

			Dxy[0].X = -1; Dxy[0].Y = -1; Dxy[1].X = -1; Dxy[1].Y = 0; Dxy[2].X = -1; Dxy[2].Y = 1;
			Dxy[3].X = 0; Dxy[3].Y = -1; Dxy[4].X = 0; Dxy[4].Y = 1;
			Dxy[5].X = 1; Dxy[5].Y = -1; Dxy[6].X = 1; Dxy[6].Y = 0; Dxy[7].X = 1; Dxy[7].Y = 1;

			EnterCells();

			FieldInit();
		}

		public void FieldInit()
		{

			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != length; y++)
				{
					if (fld[x, y].Status == StatusCell.Yes || IsAddlist(x, y))
					{
						ListCells += fld[x, y].Calc;
						fld[x, y].active = true;
					}
				}
			}
		}

		public void ClearField()
		{

			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != length; y++)
				{
					fld[x, y].SetNo();
				}
			}

			ClearListCells();
		}

		public void DrawAll(Graphics g)
		{
			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != length; y++)
				{
					fld[x, y].Draw(g);
				}
			}
		}

		public void Draw(Graphics g)
		{
			ListCells?.Invoke(new CellEvent(Calccmd.del_calc_func, g));
		}



		// проверяем, нужно ли добавить ячейку в список событий.
		// (находится ли рядом с данной ячейкой, хотя бы одна живая ячейка)
		public bool IsAddlist(int x, int y)
		{
			foreach (FieldLocation i in Dxy)
			{
				if (fld[x + i.X, y + i.Y].IsLive())
				{
					return true;
				}
			}

			return false;
		}

		// подсчёт живых ячеек вокруг данной. status - состояние вызывающей ячейки - клетка : true - есть, false - нет
		public int IsLiveCount(int x, int y, bool status)
		{

			int count = 0;   // счётчик живых ячеек первого поколения вокруг данной


			foreach (FieldLocation l in Dxy)
			{

				if (status)                               // если вызывающая клетка есть
				{
					if (!fld[x + l.X, y + l.X].active)
					{  // добавляем найденную в список событий, если ещё не добавлена
						ListCells += fld[x + l.X, y + l.Y].Calc;

						fld[x + l.X, y + l.Y].active = true;
					}
				}

				if (fld[x + l.X, y + l.Y].IsLive())     // если найденная клетка живая - увеличиваем счётчик
				{
					count++;
				}

			}

			return count;
		}

		public void OnListCells()
		{
			if (ListCells != null)
			{
				ListCells(new CellEvent(Calccmd.set_status));  // фиксируем изменения прошлого хода

				ListCells(new CellEvent(Calccmd.calc_cell));   // делаем новый ход (вычисляем)
			}
		}

		public void ClearListCells()
		{
			ListCells?.Invoke(new CellEvent(Calccmd.del_calc_func));   // удаляем все обработчики события
		}

		// Заполнение поля клетками случайным образом
		public void EnterCells()
		{
			Random rnd = new Random();

			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != length; y++)
				{

					int r = rnd.Next(300);
					if (r > 100 && r < 200)
					{
						fld[x, y].SetYes(); // клетка есть
					}
					else
					{
						fld[x, y].SetNo();  // клетки нет
					}
				}
			}
		}

	}
}
