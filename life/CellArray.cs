﻿using System;
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

                if (cells[x, y] == null && value != null)
                {
                    cells[x, y] = value;

                    Count++;
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
                Count++;
            }

            cells[cell.Location.X, cell.Location.Y] = cell;
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

        public void Resize(int dx, int dy)
        {
            cells = new Cell[dx, dy];
            Count = 0;
            width = dx;
            height = dy;
        }
    }
}
