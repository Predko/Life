using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace life
{
	// Индексатор с круговыми индексами

	public class CellArray
	{
		private Cell[,] cells;
		protected readonly int width;
		protected readonly int height;



		public CellArray(Field fld, int x, int y)
		{
			width = x;
			height = y;

			Init(fld);
		}

		private void Init(Field fld)
		{
			cells = new Cell[width, height];
			// инициализация всех ячеек
			for (int k = 0; k != width; k++)
			{
				for (int m = 0; m != height; m++)
				{
					cells[k, m] = new Cell(fld, k, m);
				}
			}
		}

		// индексатор
		public Cell this[int i, int j]
		{
			get
			{
				return cells[TrueIndexI(i), TrueIndexJ(j)];
			}
			set
			{
				cells[TrueIndexI(i), TrueIndexJ(j)].Copy(value);
			}
		}

		// возвращает правильный индекс i = 0 - (mi-1)
		protected virtual int TrueIndexI(int i)
		{
			if (i < 0)
			{
				return width + i;
			}
			else
			if (i >= width)
			{
				return i - width;
			}

			return i;
		}

		// возвращает правильный индекс j = 0 - (mj-1)
		protected virtual int TrueIndexJ(int j)
		{
			if (j < 0)
			{
				return height + j;
			}
			else
			if (j >= height)
			{
				return j - height;
			}

			return j;
		}
	}
}
