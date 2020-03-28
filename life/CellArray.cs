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
        
        public CellArray(int x, int y)
        {
            cells = new Cell[x, y];
            Count = 0;
        }
        
        public Cell this[int x, int y]
        {
            get => cells[x, y];

            set
            {
                if (cells[x, y] == null)
                {
                    cells[x, y] = value;

                    Count++;
                }
            }
        }

        public void Add(Cell cell)
        {
            if (cell == null)
                return;

            cells[cell.Location.X, cell.Location.Y] = cell;
            Count++;
        }

        public void Remove(Cell cell)
        {
            cells[cell.Location.X, cell.Location.Y] = null;
            Count--;
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            int count = Count;

            foreach (Cell cell in cells)
            {
                if (cell == null)
                    continue;

                if (count-- == 0)
                {
                    yield break;
                }

                yield return cell;
            }
            
            //for (int x = 0; x < cells.GetLength(0); x++)
            //{
            //    for (int y = 0; y < cells.GetLength(1); y++)
            //    {
                    
                    
            //        yield return cells[x,y];
            //    }
            //}
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
