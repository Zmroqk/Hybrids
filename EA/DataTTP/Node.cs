using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public class Node
    {
        public Node()
        {
            this.Index = 0;
            this.X = 0;
            this.Y = 0;
            this.AvailableItems = new List<Item>();
        }

        public Node(int index, double x, double y)
        {
            this.Index = index;
            this.X = x;
            this.Y = y;
            this.AvailableItems = new List<Item>();
        }

        public int Index { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public List<Item> AvailableItems { get; set; }
    }
}
