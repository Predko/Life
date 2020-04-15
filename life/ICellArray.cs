using System.Collections.Generic;

namespace life
{
    public interface ICellArray: IEnumerable<Cell>
    {
        Cell this[int x, int y] { get; set; }

        int Count { get; set; }

        void Add(Cell cell);
        void Remove(Cell cell);
        void Clear();
        void Resize(int dx, int dy);
    }
}