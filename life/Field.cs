﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
		private readonly int width;
		private readonly int height;

		public Size CellSize { get; set; }

		public Brush brushCellYes;
		public Brush brushCellNo; 

		// массив координат ячеек вокруг данной
		private readonly FieldLocation[] Dxy = new FieldLocation[8];

		public CellArray fld;

		public readonly List<FieldLocation> listCellLocationToDraw;

		public event CalcCell ListCells;


		// Длина и ширина в ячейках игрового поля
		public Field(int width, int height)
		{
			this.height = height;
			this.width = width;
			fld = new CellArray(this, width, height);

			Dxy[0].X = -1; Dxy[0].Y = -1; Dxy[1].X = -1; Dxy[1].Y = 0; Dxy[2].X = -1; Dxy[2].Y = 1;
			Dxy[3].X = 0; Dxy[3].Y = -1; Dxy[4].X = 0; Dxy[4].Y = 1;
			Dxy[5].X = 1; Dxy[5].Y = -1; Dxy[6].X = 1; Dxy[6].Y = 0; Dxy[7].X = 1; Dxy[7].Y = 1;

			

			listCellLocationToDraw = new List<FieldLocation>();
		}

		public void CalcNextStep()
		{
			if (ListCells != null)
			{
				ListCells?.Invoke(new CellEvent(Calccmd.analysis_nextstep));    // рассчитываем состояние клеток для следующего шага
				ListCells?.Invoke(new CellEvent(Calccmd.apply_changes));		// фиксируем рассчитанные изменения
				ListCells?.Invoke(new CellEvent(Calccmd.delete_not_active));    // удаляем неактивные и добавляем близлежащие
			}
		}

		public void DrawAll()
		{
			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != height; y++)
				{
					listCellLocationToDraw.Add(fld[x, y].Location);
				}
			}
		}

		public void DrawAll(Graphics g)
		{
			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != height; y++)
				{
					fld[x, y].Draw(g);
				}
			}
		}

		public void Draw(Graphics g)
		{
			foreach (var cl in listCellLocationToDraw)
			{
				fld[cl.X, cl.Y].Draw(g);
			}

			listCellLocationToDraw.Clear();
		}

		// проверяем, нужно ли добавить ячейку в список событий.
		// (находится ли рядом с данной ячейкой, хотя бы одна живая ячейка)
		public bool IsAddlist(int x, int y)
		{
			foreach (FieldLocation i in Dxy)
			{
				Cell currentCell = fld[x + i.X, y + i.Y];
				if (!currentCell.isStaticCell && currentCell.IsLive())
				{
					return true;
				}
			}

			return false;
		}

		// - подсчёт живых ячеек вокруг данной. status - состояние вызывающей ячейки - клетка : true - есть, false - нет
		// - 
		public int NumberLiveCells(int x, int y, bool status)
		{

			int count = 0;   // счётчик живых ячеек первого поколения вокруг данной

			foreach (FieldLocation loc in Dxy)
			{
				Cell currentcell = fld[x + loc.X, y + loc.Y];

				if (currentcell.isStaticCell)	// Статичная клетка(стенка)
				{
					return 5;	// клетки рядом с ней должны погибнуть
				}

				if (status && !currentcell.active)		// если вызывающая клетка есть и выбранная клетка не активна(не в списке событий)
				{
						ListCells += currentcell.Calc;	// добавляем найденную в список событий

						currentcell.active = true;		// клетка в списке событий
				}

				if (currentcell.IsLive())     // если найденная клетка живая - увеличиваем счётчик
				{
					count++;
				}
			}

			return count;
		}

		public void FieldInit()
		{
			for (int x = 0; x != width; x++)
			{
				for (int y = 0; y != height; y++)
				{
					if (!fld[x, y].isStaticCell 
						&& (fld[x, y].Status == StatusCell.Yes || IsAddlist(x, y)))
					{
						ListCells += fld[x, y].Calc;
						fld[x, y].active = true;
					}
				}
			}
		}

		public void GospersGliderGun(int x0, int y0)
		{
			long[] bitmap = new long[] {
				36,
				0b000000000000000000000000100000000000,
				0b000000000000000000000010100000000000,
				0b000000000000110000001100000000000011,
				0b000000000001000100001100000000000011,
				0b110000000010000010001100000000000000,
				0b110000000010001011000010100000000000,
				0b000000000010000010000000100000000000,
				0b000000000001000100000000000000000000,
				0b000000000000110000000000000000000000
			};

			PlaceLifeObject(x0, y0, bitmap);
		}

		public void DiagonalSpaceShip(int x0, int y0)
		{
			long[] bitmap = new long[] {
				22,
				0b0000000000110000000000,
				0b0000000001001000000000,
				0b0000000011000000000000,
				0b0000000001011000000000,
				0b0000000000101110000000,
				0b0000000000110111000000,
				0b0000000000001000011000,
				0b0000000000001110000110,
				0b0010000000001010000000,
				0b0111000000001001000000,
				0b1000110000000000000000,
				0b1001010000000110100100,
				0b0101101111000100011110,
				0b0000110100011000000010,
				0b0000110110010000000001,
				0b0000010001000000001011,
				0b0000000000010000000100,
				0b0000001000001000000100,
				0b0000001000001001000000,
				0b0000000100011000110000,
				0b0000000100001101000000,
				0b0000000000000011000000
			};

			PlaceLifeObject(x0, y0, bitmap);
		}

		private void PlaceLifeObject(int x0, int y0, long[] bitmap)
		{
			int x1 = x0 + (int)bitmap[0] - 1;
			int y1 = y0 + bitmap.Length - 1;
			int i = 1;

			for(int y = y0; y < y1; y++, i++)
			{
				long current = bitmap[i];
				int x = x1;

				for( int j = (int)bitmap[0]; j > 0; j--, x--)
				{
					if((current & 1) == 0)
					{
						fld[x, y].SetCellNo();
					}
					else
					{
						fld[x, y].SetCellYes();
					}

					current >>= 1;
				}
			}
		}



		// Заполнение поля клетками случайным образом
		public void EnterCells()
		{
			//Random rnd = new Random();

			//for (int x = 0; x != width; x++)
			//{
			//	for (int y = 0; y != height; y++)
			//	{

			//		int r = rnd.Next(300);
			//		if (r > 100 && r < 200)
			//		{
			//			fld[x, y].SetCellYes(); // клетка есть
			//		}
			//		else
			//		{
			//			fld[x, y].SetCellNo();  // клетки нет
			//		}
			//	}
			//}

			for (int x = 0; x < width; x++)
			{
				fld[x, 0].SetStaticCell();
				fld[x, height - 1].SetStaticCell();
			}

			for (int y = 1; y < height - 1; y++)
			{
				fld[0, y].SetStaticCell();
				fld[width - 1, y].SetStaticCell();
			}

			GospersGliderGun(5, 5);

			//DiagonalSpaceShip(width - 30, height - 30);
		}
	}
}
