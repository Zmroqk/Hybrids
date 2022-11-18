using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public class Data
    {
        public Data()
        {
            this.Nodes = new List<Node>();
            this.Items = new List<Item>();
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public int CityCount { get; set; }
        public int NumberOfItems { get; set; }
        public int KnapsackCapacity { get; set; }
        public double MinSpeed { get; set; }
        public double MaxSpeed { get; set; }
        public double RentingRatio { get; set; }
        public string EdgeWeightType { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Item> Items { get; set; }

        public DistanceInfo[][] Distances { get; set; }

        public double? Median { get; set; }

        public DistanceInfo[][] GetNodeMatrix()
        {
            return this.Distances;       
        }

        public void GenerateDistances()
        {
            this.Distances = new DistanceInfo[this.CityCount][];
            foreach (var node in Nodes)
            {
                this.Distances[node.Index - 1] = new DistanceInfo[this.CityCount];
                foreach (var otherNode in Nodes)
                {
                    var distance = Math.Sqrt(Math.Pow(node.X - otherNode.X, 2) + Math.Pow(node.Y - otherNode.Y, 2));
                    this.Distances[node.Index - 1][otherNode.Index - 1] = new DistanceInfo()
                    {
                        Distance = distance,
                        From = node,
                        To = otherNode
                    };
                }
            }
        }

        public double GetDistance(Node first, Node second)
        {
            return this.Distances[first.Index - 1][second.Index - 1].Distance;
        }

        public double SortedItemMedian()
        {
            if (!this.Median.HasValue)
            {
                var SortedItems = this.Items.ToList();
                SortedItems.Sort((i1, i2) =>
                {
                    var rate1 = (double)i1.Profit / i1.Weight;
                    var rate2 = (double)i2.Profit / i2.Weight;
                    if (rate1 < rate2)
                    {
                        return 1;
                    }
                    else if (rate1 == rate2)
                    {
                        return 0;
                    }
                    return -1;
                });
                var rates = SortedItems.Select(i => (double)i.Profit / i.Weight).ToList();
                this.Median = rates.Count % 2 == 0 ?
                    rates[(rates.Count / 2) - 1] :
                    (rates[(rates.Count / 2)] + rates[(rates.Count / 2) - 1]) / 2;
            }
            return this.Median.Value;
        }
    }
}
