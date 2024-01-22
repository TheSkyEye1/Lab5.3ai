using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab5._3
{
    public class CollectableObject
    {
        public int ID;
        public Point point;
        public int radius;

        public CollectableObject(int ID, Point point, int radius)
        {
            this.ID = ID;
            this.point = point;
            this.radius = radius;
        }
    }
}
