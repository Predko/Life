using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace life
{
    class CellArray : ICellArray
    {
        public int Count { get; set; }

        private Cell[,] cells;

        private int width;
        private int height;

        public CellArray(int x, int y)
        {
            Resize(x, y);
        }

        public Cell this[int x, int y]
        {
            get
            {
                if (x >= width || y >= height || x < 0 || y < 0)
                {
                    return null;
                }

                return cells[x, y];
            }

            set
            {
                if (x >= width || y >= height || x < 0 || y < 0)
                {
                    return;
                }

                if (cells[x, y] == null)
                {
                    if (value != null)
                    {
                        // Записываем новую клетку в пустую ячейку.
                        cells[x, y] = value;

                        Count++;
                    }
                }
                else
                {
                    if (value != null)
                    {
                        // Меняем уже имеющуюся клетку.
                        cells[x, y].Copy(value);
                    }
                    else
                    {
                        // Обнуляем(удаляем) имеющуюся клетку из массива.
                        cells[x, y] = value;

                        Count--;
                    }
                }
            }
        }

        public void Add(Cell cell)
        {
            if (cell == null)
            {
                return;
            }

            if (cell.Location.X >= width || cell.Location.Y >= height || cell.Location.X < 0 || cell.Location.Y < 0)
            {
                return;
            }

            if (cells[cell.Location.X, cell.Location.Y] == null)
            {
                cells[cell.Location.X, cell.Location.Y] = cell;

                Count++;
            }
            else
            {
                cells[cell.Location.X, cell.Location.Y].Copy(cell);
            }
        }

        public void Remove(Cell cell)
        {

            if (cell.Location.X >= width || cell.Location.Y >= height || cell.Location.X < 0 || cell.Location.Y < 0)
            {
                return;
            }

            if (cells[cell.Location.X, cell.Location.Y] != null)
            {
                cells[cell.Location.X, cell.Location.Y] = null;

                Count--;
            }
        }

        public void Clear()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = null;
                }
            }

            Count = 0;
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            foreach (Cell cell in cells)
            {
                if (cell == null)
                    continue;

                yield return cell;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Resize(int dx, int dy)
        {
            cells = new Cell[dx, dy];
            Count = 0;
            width = dx;
            height = dy;
        }
    }
}
