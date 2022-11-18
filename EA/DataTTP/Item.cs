using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public class Item
    {
        public Item()
        {
            this.Index = 0;
            this.Profit = 0;
            this.Weight = 0;
            this.NodeIndex = 0;
        }
        public Item(int index, int profit, int weight, int nodeIndex, Node node)
        {
            this.Index = index;
            this.Profit = profit;
            this.Weight = weight;
            this.NodeIndex = nodeIndex;
            this.Node = node;
        }

        public int Index { get; set; }
        public int Profit { get; set; }
        public int Weight { get; set; }
        public int NodeIndex { get; set; }
        public Node Node { get; set; }
    }
}
