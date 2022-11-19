using EA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Mutators
{
    public class KnapsackMutatorBase<TSpecimen> : IMutator<TSpecimen> where TSpecimen : TTPSpecimenBase<TSpecimen>
    {
        public double Probability { get => 1; set => _ = value; }
        public bool UseGreedy { get; set; }
        public Data Config { get; set; }
        protected Random random;

        public KnapsackMutatorBase(Data config, bool useGreedy = true)
        {
            this.Config = config;
            this.UseGreedy = useGreedy;
            this.random = new Random();
        }


        public TSpecimen Mutate(TSpecimen specimen)
        {
            if (this.UseGreedy)
            {
                this.GreedyMutate(specimen);
            }
            else
            {
                this.RandomMutate(specimen);
            }
            return specimen;
        }

        private List<Item> GenerateSortedItemList(TSpecimen specimen)
        {
            var nodes = specimen.Nodes.ToList();
            var itemsDict = new List<(Item item, double distance)>();
            foreach(var node in specimen.Nodes)
            {
                nodes.Remove(node);
                var distance = GetRemainingDistance(node, nodes);
                foreach(var item in node.AvailableItems)
                {
                    itemsDict.Add((item, distance));
                }
            }
            itemsDict.Sort((i1, i2) =>
            {
                var rate1 = ((double)i1.item.Profit / i1.item.Weight) / i1.distance;
                var rate2 = ((double)i2.item.Profit / i2.item.Weight) / i2.distance;
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
            return itemsDict.Select(x => x.item).ToList();
        }

        private double GetRemainingDistance(Node current, List<Node> remaining)
        {
            double distance = 0d;
            foreach(var node in remaining)
            {
                distance += this.Config.Distances[current.Index-1][node.Index-1].Distance;
                current = node;
            }
            return distance;
        }

        private void GreedyMutate(TSpecimen specimen)
        {
            specimen.RemoveAllItemsFromKnapsack();
            var items = GenerateSortedItemList(specimen);
            while (items.Count > 0 && specimen.AddItemToKnapsack(items.First()))
            {
                items.RemoveAt(0);
            }
        }

        private void RandomMutate(TSpecimen specimen)
        {
            specimen.RemoveAllItemsFromKnapsack();
            while (specimen.AddItemToKnapsack(this.Config.Items[random.Next(this.Config.Items.Count)]))
            {
            }
        }

        public IList<TSpecimen> MutateAll(IList<TSpecimen> currentPopulation)
        {
            throw new NotImplementedException();
        }
    }
}
