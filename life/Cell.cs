using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace life
{
	// кординаты ячеёки
	public struct Coord
	{
		public int x, y;
	}

	// отрисовка клетки
	interface IDraw
	{
		void Draw();
	}


	public enum Calccmd { set_status, calc_cell, del_calc_func };
	public enum Cell_is { yes, no };

	public enum StatusCell { Yes, No };


	public class Cell : IDraw
	{
		public StatusCell Status { get; private set; } // символ L  или E - есть клетка или нет
		public bool gen;                // поколение клетки: true - первое, false - нулевое
		public bool ischange;       // изменение клетки: true - должно измениться, false - нет
		public bool active;         // активная ячейка(true)- добавлена в список событий
		public int X { get; set; }          // координаты ячейки
		public int Y { get; set; }           // координаты ячейки

		public const char L = '0';  // Клетка есть
		public const char E = ' ';  // Клетки нет

		private readonly Field Field;

		public bool IsLive()
		{
			return gen && Status == StatusCell.Yes; // клетка первого поколения(не только что созданная) и клетка живая
		}


		public Cell(Field fld, int px = 0, int py = 0)
		{
			Field = fld;
			SetCell();
			X = px; Y = py;
		}


		public void Copy(Cell cl)
		{
			SetCell(cl.Status);
			X = cl.X; Y = cl.Y;
		}

		public void SetCell(StatusCell cl = StatusCell.No)
		{
			Status = cl;
			gen = true;
			ischange = false;
			active = false;
		}

		public void SetNo()
		{
			SetCell(StatusCell.No);
		}

		public void SetYes()
		{
			SetCell(StatusCell.Yes);
		}

		// отрисовываем ячейку
		public virtual void Draw()
		{
			Console.SetCursorPosition(X, Y);
			Console.Write((Status == StatusCell.Yes) 
							? '*' 
							: ' ');
		}

		// рассчитываем следующий ход ячейки(жива/нет) или меняем статус клетки если cng == true,
		// удаляется из списка событий, если нет,
		// удаляется из списка событий, если рядом нет живых ячеек первого поколения,
		// если рядом больше двух живых ячеек первого поколения, отмечает себя живой, проверяем соседние
		public void Calc(Calccmd cng)
		{

			if (cng == Calccmd.set_status)
			{
				if (ischange)
				{
					if (Status == StatusCell.No)
					{
						Status = StatusCell.Yes;                          // клетка появится
						Field.IsLiveCount(X, Y, IsLive());  // добавляем окружающие в список события
					}
					else
						Status = StatusCell.No;                          // клетка исчезнет

					Draw();
					ischange = false;
				}
				return;
			}
			else if (cng == Calccmd.del_calc_func)
			{
				Field.ListCells -= Calc;
				return;
			}


			int countcells = Field.IsLiveCount(X, Y, IsLive());

			if (countcells == 3)
			{
				if (Status == StatusCell.No)         // клетки нет
					ischange = true;    // клетка рождается
			}
			else if (countcells != 2)
			{

				if (Status == StatusCell.Yes)         // клетка есть
					ischange = true;    // клетка исчезает 
				else
					if (countcells == 0)
				{ // клетки нет и вокруг нет живых - удаляем из списка события
					Field.ListCells -= Calc;
					active = false;
				}
			}
		}
	}
}
