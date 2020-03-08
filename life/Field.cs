using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace life
{
	public delegate void CalcCell(Calccmd cng);

	// поле, верхняя часть соединена с нижней, левая с правой
	public class Field
	{
		private readonly int length;
		private readonly int width;

		// массив координат ячеек вокруг данной
		private readonly Coord[] Cxy = new Coord[8];

		public CellArray fld;


		public event CalcCell ListCells;


		public Field()
		{
			length = Console.WindowHeight - 1;
			width = Console.WindowWidth;
			fld = new CellArray(this, width, length);

			Cxy[0].x = -1; Cxy[0].y = -1; Cxy[1].x = -1; Cxy[1].y = 0; Cxy[2].x = -1; Cxy[2].y = 1;
			Cxy[3].x = 0; Cxy[3].y = -1; Cxy[4].x = 0; Cxy[4].y = 1;
			Cxy[5].x = 1; Cxy[5].y = -1; Cxy[6].x = 1; Cxy[6].y = 0; Cxy[7].x = 1; Cxy[7].y = 1;

			EnterCells();

			FieldInit();
		}

		public void FieldInit()
		{

			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != length; y++)
				{
					fld[x, y].Draw();

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
					fld[x, y].Draw();
				}
			}

			ClearListCells();
		}

		// проверяем, находится ли рядом с данной ячейкой, хотя бы одна живая ячейка.
		public bool IsAddlist(int x, int y)
		{

			foreach (Coord i in Cxy)
			{
				if (fld[x + i.x, y + i.y].IsLive())
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


			foreach (Coord i in Cxy)
			{

				if (status)                               // если вызывающая клетка есть
				{
					if (!fld[x + i.x, y + i.y].active)
					{  // добавляем найденную в список событий, если ещё не добавлена
						ListCells += fld[x + i.x, y + i.y].Calc;

						fld[x + i.x, y + i.y].active = true;
					}
				}

				if (fld[x + i.x, y + i.y].IsLive())     // если найденная клетка живая - увеличиваем счётчик
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
				ListCells(Calccmd.set_status);  // фиксируем изменения прошлого хода

				ListCells(Calccmd.calc_cell);   // делаем новый ход (вычисляем)
			}
		}

		public void ClearListCells()
		{
			ListCells?.Invoke(Calccmd.del_calc_func);   // удаляем все обработчики события
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
