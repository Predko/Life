using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    public static class PointExtentionMethods
    {
        public static bool Less (this Point p, Point p1)
        {
            return (p.X < p1.X || p.Y < p1.Y);
        }
    }
}
