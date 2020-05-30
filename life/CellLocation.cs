using System;

namespace life
{
    public struct CellLocation : IComparable<CellLocation>, IEquatable<CellLocation>
    {
        public int X;
        public int Y;

        static public CellLocation Empty = new CellLocation(0, 0);

        public CellLocation(int x, int y)
        {
            X = x;
            Y = y;
        }



        public int CompareTo(CellLocation fl) => GetHashCode() - fl.GetHashCode();

        public override bool Equals(object obj) => (obj is CellLocation fl && Equals(fl));

        public bool Equals(CellLocation fl) => (GetHashCode() == fl.GetHashCode());

        public override int GetHashCode() => ((Y & 0x7FFF) << 15) | (X & 0x7FFF);

        public static bool operator ==(CellLocation left, CellLocation right) => left.Equals(right);

        public static bool operator !=(CellLocation left, CellLocation right) => !(left == right);

        public static bool operator <(CellLocation left, CellLocation right) => left.CompareTo(right) < 0;

        public static bool operator <=(CellLocation left, CellLocation right) => left.CompareTo(right) <= 0;

        public static bool operator >(CellLocation left, CellLocation right) => left.CompareTo(right) > 0;

        public static bool operator >=(CellLocation left, CellLocation right) => left.CompareTo(right) >= 0;
    }
}
