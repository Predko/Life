﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace life
{
	// Индексатор с круговыми индексами

	public class CellArray
	{
		readonly Cell[,] cells;
		private readonly int mi;
		private readonly int mj;



		public CellArray(Field fld, int i, int j)
		{
			cells = new Cell[i, j];
			// инициализация всех ячеек
			for (int k = 0; k != i; k++)
			{
				for (int m = 0; m != j; m++)
				{
					cells[k, m] = new Cell(fld, k, m);
				}
			}

			mi = i;
			mj = j;
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
		int TrueIndexI(int i)
		{
			if (i < 0)
			{
				return mi + i;
			}
			else
			if (i >= mi)
			{
				return i - mi;
			}

			return i;
		}

		// возвращает правильный индекс j = 0 - (mj-1)
		int TrueIndexJ(int j)
		{
			if (j < 0)
			{
				return mj + j;
			}
			else
			if (j >= mj)
			{
				return j - mj;
			}

			return j;
		}
	}
}
