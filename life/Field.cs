using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace life
{
	public struct FieldLocation: IComparable<FieldLocation>, IEquatable<FieldLocation>
	{
		public short X;
		public short Y;

		public FieldLocation(int x, int y)
		{
			X = (short)(x & 0x7FFF);
			Y = (short)(y & 0x7FFF);
		}

		public int CompareTo(FieldLocation fl) => GetHashCode() - fl.GetHashCode();

		public override bool Equals(object obj) => (obj is FieldLocation fl && Equals(fl));

		public bool Equals(FieldLocation fl) => (GetHashCode() == fl.GetHashCode());

		public override int GetHashCode() => ((Y & 0x7FFF) << 15) | (X & 0x7FFF);

		public static bool operator ==(FieldLocation left, FieldLocation right) => left.Equals(right);

		public static bool operator !=(FieldLocation left, FieldLocation right) => !(left == right);

		public static bool operator <(FieldLocation left, FieldLocation right)	=> left.CompareTo(right) < 0;

		public static bool operator <=(FieldLocation left, FieldLocation right) => left.CompareTo(right) <= 0;

		public static bool operator >(FieldLocation left, FieldLocation right)	=> left.CompareTo(right) > 0;

		public static bool operator >=(FieldLocation left, FieldLocation right) => left.CompareTo(right) >= 0;

		//public static implicit operator FieldLocation(Point p)
		//{
		//	return new FieldLocation() { X = p.X, Y = p.Y };
		//}

	}

	// поле, верхняя часть соединена с нижней, левая с правой
	public class Field
	{
		private readonly short width;
		private readonly short height;

		public Size CellSize { get; set; }
		public Rectangle rectangle { get; set; }

		public Brush brushCellYes;
		public Brush brushCellNo;

		public Bitmap bitmap;
		public Bitmap bitmapCellYesNo;

		public Rectangle rectCellYes;
		public Rectangle rectCellNo;

		// массив координат ячеек вокруг данной
		private readonly FieldLocation[] Dxy = new FieldLocation[8];

		public BinaryTreeCells field;

		public List<Cell> CurrentListCells;         // Список активных клеток текущего хода
		public List<Cell> NewListCells;             // Список активных клеток следующего хода

		public string log;  // строка содержащая информацию о изменениях в состоянии активных клеток на текущем ходе
							// в виде: +(x,y) - клетка появилась, -(x,y) - клетка исчезла. например: +(1,1);-(10,5)
							// <текущий шаг>:<+/->(<X>,<Y>)[;]



		// Длина и ширина в ячейках игрового поля. 
		// Максимальные размер ограничен максимальным положительным значением типа short
		public Field(int width, int height)
		{
			this.height = (short)(height & 0x7fff);
			this.width = (short)(width & 0x7fff);

			field = new BinaryTreeCells();

			Dxy[0].X = -1; Dxy[0].Y = -1; Dxy[1].X = -1; Dxy[1].Y = 0; Dxy[2].X = -1; Dxy[2].Y = 1;
			Dxy[3].X = 0; Dxy[3].Y = -1; Dxy[4].X = 0; Dxy[4].Y = 1;
			Dxy[5].X = 1; Dxy[5].Y = -1; Dxy[6].X = 1; Dxy[6].Y = 0; Dxy[7].X = 1; Dxy[7].Y = 1;

			CurrentListCells = new List<Cell>();
		}

		public void InitBitmap()
		{
			bitmap = new Bitmap(rectangle.Width, rectangle.Height);

			bitmapCellYesNo = new Bitmap(CellSize.Width * 2, CellSize.Height);

			DrawFieldToBitmap();
		}

		private void DrawFieldToBitmap()
		{
			Graphics bitmapGraphics = Graphics.FromImage(bitmapCellYesNo);

			rectCellYes = new Rectangle(0, 0, CellSize.Width, CellSize.Height);
			rectCellNo = new Rectangle(CellSize.Width, 0, CellSize.Width, CellSize.Height);

			bitmapGraphics.FillRectangle(brushCellYes, rectCellYes);

			bitmapGraphics.FillRectangle(brushCellNo, rectCellNo);

			bitmapGraphics.Dispose();

			bitmapGraphics = Graphics.FromImage(bitmap);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Rectangle rectCurrentCell = new Rectangle()
													{
														X = x * CellSize.Width + 1,
														Y = y * CellSize.Height + 1,
														Width = CellSize.Width - 2,
														Height = CellSize.Height - 2
													};


					bitmapGraphics.DrawImage(bitmapCellYesNo, rectCurrentCell, rectCellNo, GraphicsUnit.Pixel);
				}
			}

			// Временно меняем прямоугольник отрисовки игрового поля на прямоугольник bitmap
			Rectangle frect = rectangle;

			rectangle = new Rectangle(0, 0, rectangle.Width, rectangle.Height);

			foreach(var cell in field)
			{
				if(cell.isStaticCell)
				{
					cell.Draw(bitmapGraphics);
				}
			}

			// Восстанавливаем
			rectangle = frect;

			bitmapGraphics.Dispose();
		}

		public void Draw(Graphics g)
		{
			Rectangle rectsrc = new Rectangle(0, 0, rectangle.Width, rectangle.Height);
			
			g.DrawImage(bitmap, rectangle, rectsrc, GraphicsUnit.Pixel);
			
			foreach (var cell in field)
			{
				cell.Draw(g);
			}
		}

		public void AddCell(Cell cell) => field.Add(cell);

		public void RemoveCell(Cell cell)
		{
			if (!cell.isStaticCell)	// не удаляем из списка статичные клетки
			{
				field.Remove(cell);
			}
		}

		// Добавляем близлежащие клетки к активной в список активных клеток
		public void AddNearestCells(short x, short y)
		{
			foreach (FieldLocation i in Dxy)
			{
				Cell currentCell = field[x + i.X, y + i.Y];

				if (currentCell == null)
				{
					currentCell = new Cell(this, new FieldLocation(x + i.X, y + i.Y)) { active = true };
					
					field[x + i.X, y + i.Y] = currentCell;

					CurrentListCells.Add(currentCell);
				}
				else 
				if (currentCell.isStaticCell)
				{
					continue;
				}
			}
		}

		// - подсчёт живых ячеек вокруг данной. status - состояние вызывающей ячейки - клетка : true - есть, false - нет
		public int NumberLiveCells(short x, short y)
		{

			int count = 0;   // счётчик живых ячеек вокруг данной

			foreach (FieldLocation loc in Dxy)
			{
				Cell currentcell = field[x + loc.X, y + loc.Y];

				if (currentcell == null)
					continue;					// клетки нет

				if (currentcell.isStaticCell)	// Статичная клетка(стенка)
				{
					return 5;	// клетки рядом с ней должны погибнуть
				}

				if (currentcell.IsLive())     // если найденная клетка живая - увеличиваем счётчик
				{
					count++;
				}
			}

			return count;
		}

		public void CalcNextStep()
		{
			NewListCells.Clear();

			// рассчитываем состояние клеток для следующего шага
			// Заносим клетки в список клеток следующего шага - NewListCells
			foreach (Cell cell in CurrentListCells)
			{
				cell.AnalysisNextStep();
			}

			foreach (Cell cell in CurrentListCells)
			{
				cell.ChangeStatus();	// фиксируем изменения клеток
			}

			CurrentListCells.Clear();
			CurrentListCells.AddRange(NewListCells);
			
			foreach (Cell cell in NewListCells)
			{
				AddNearestCells(cell.Location.X, cell.Location.Y);
				Console.WriteLine($"{cell.Location.X},{cell.Location.Y}");
			}
			Console.WriteLine($"{NewListCells.Count}\t{CurrentListCells.Count}\t{field.Count}\n");
		}

		public void FieldInit()
		{
			NewListCells = new List<Cell>();

			foreach(Cell currentCell in field)
			{
				if (!currentCell.isStaticCell && currentCell.IsLive())
				{
					currentCell.active = true;
					NewListCells.Add(currentCell);
				}
			}

			CurrentListCells.Clear();
			CurrentListCells.AddRange(NewListCells);

			foreach (Cell cell in NewListCells)
			{
				AddNearestCells(cell.Location.X, cell.Location.Y);
			}
		}

		public void GospersGliderGun(short x0, short y0)
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

				for(int j = (int)bitmap[0]; j > 0; j--, x--)
				{
					if((current & 1) != 0)
					{
						Cell cell = new Cell(this, x, y) { active = true, Status = StatusCell.Yes };

						field[x, y] = cell;
					}
					
					current >>= 1;
				}
			}
		}

		// Заполнение поля клетками случайным образом
		public void EnterCells()
		{
			Random rnd = new Random();

			for (int x = 2; x < width - 2; x++)
			{
				for (int y = 2; y < height - 2; y++)
				{
					int r = rnd.Next(300);
					if (r > 100 && r < 200)
					{
						field[x, y] = new Cell(this, x, y) { Status = StatusCell.Yes, active = true }; // клетка есть
					}
				}
			}

			for (int x = 0; x < width; x++)
			{
				field[x, 0] = new Cell(this, x, 0) { isStaticCell = true, Status = StatusCell.Yes };
				field[x, height - 1] = new Cell(this, x, height - 1) { isStaticCell = true, Status = StatusCell.Yes };
			}

			for (int y = 1; y < height - 1; y++)
			{
				field[0, y] = new Cell(this, 0, y) { isStaticCell = true, Status = StatusCell.Yes };
				field[width - 1, y] = new Cell(this, width - 1, y) { isStaticCell = true, Status = StatusCell.Yes };
			}

			// GospersGliderGun(5, 5);

			//DiagonalSpaceShip(width - 30, height - 30);
		}
	}
}
