using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public abstract class TTPSpecimenBase<TSpecimen> : ISpecimen<TSpecimen>, IEquatable<TSpecimen> where TSpecimen : ISpecimen<TSpecimen>
    {
        public Data Config { get; set; }

        public TTPSpecimenBase(Data config, ISpecimenInitializator<TSpecimen> specimenInitialization)
        {
            this.Nodes = new List<Node>();
            this.Config = config;
            this.Items = new HashSet<Item>();
            this.SpecimenInitialization = specimenInitialization;
            this.CurrentKnapsackUsage = 0;
            this.EvaluationValue = null;
        }

        protected HashSet<Item> Items { get; set; }

        public List<Node> Nodes { get; set; }

        public ISpecimenInitializator<TSpecimen> SpecimenInitialization { get; }

        public double CurrentKnapsackUsage { get; set; }

        public double? EvaluationValue { get; set; }

        public bool IsCrossed { get; set; }

        public bool IsMutated { get; set; }

        public bool IsModified => this.IsCrossed || this.IsMutated;

        public double Evaluate()
        {
            if (this.EvaluationValue.HasValue)
            {
                return this.EvaluationValue.Value;
            }

            var profit = 0d;
            foreach (var item in this.Items)
            {
                profit += item.Profit;
            }
            var time = 0d;
            var currentWeight = 0d;
            this.UpdateWeight(ref currentWeight, this.Nodes[0]);
            for (int i = 1; i < this.Nodes.Count; i++)
            {
                var distance = this.Config.Distances[this.Nodes[i - 1].Index - 1][this.Nodes[i].Index - 1].Distance;
                var currentSpeed = this.Config.MaxSpeed - currentWeight * ((this.Config.MaxSpeed - this.Config.MinSpeed) / this.Config.KnapsackCapacity);
                time += distance / currentSpeed;
                this.UpdateWeight(ref currentWeight, this.Nodes[i]);
            }
            this.EvaluationValue = profit - time;
            return profit - time;
        }

        public bool AddItemToKnapsack(Item item)
        {
            if (this.Config.KnapsackCapacity >= this.CurrentKnapsackUsage + item.Weight)
            {
                this.Items.Add(item);
                this.CurrentKnapsackUsage += item.Weight;
                return true;
            }
            return false;
        }

        public Item[] GetKnapsackItems()
        {
            return this.Items.ToArray();
        }

        public void RemoveItemFromKnapsack(Item item)
        {
            this.Items.Remove(item);
            this.CurrentKnapsackUsage -= item.Weight;
        }

        public void RemoveAllItemsFromKnapsack()
        {
            this.Items = new HashSet<Item>();
            this.CurrentKnapsackUsage = 0;
        }

        public bool CheckIfItemIsInKnapsack(Item item)
        {
            return this.Items.Contains(item);
        }

        private void UpdateWeight(ref double weight, Node node)
        {
            for (int i = 0; i < node.AvailableItems.Count; i++)
            {
                var item = node.AvailableItems[i];
                if (this.Items.Contains(item))
                {
                    weight += item.Weight;
                }
            }
        }

        public void Fix()
        {
            this.EvaluationValue = null;
            var cities = new HashSet<Node>(this.Config.Nodes);
            foreach (var node in this.Nodes)
            {
                cities.Remove(node);
            }
            if (cities.Count == 0)
            {
                return;
            }
            var citiesToRemove = this.Nodes.GroupBy(n => n).Where(g => g.Count() > 1).SelectMany(g => g.Where((n, i) => i >= 1).ToList()).ToList();
            var index = 0;
            while (citiesToRemove.Count > 0)
            {
                var cityToRemove = citiesToRemove.First();
                var cityToAdd = cities.First();
                var nodeIndex = this.Nodes.IndexOf(cityToRemove);
                this.Nodes.RemoveAt(nodeIndex);
                this.Nodes.Insert(nodeIndex, cityToAdd);
                citiesToRemove.Remove(cityToRemove);
                cities.Remove(cityToAdd);
            }
        }

        public void Init()
        {
            this.SpecimenInitialization.Initialize((TSpecimen)(ISpecimen<TSpecimen>)this);
        }

        public abstract TSpecimen Clone();

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TSpecimen)
            {
                return this.Equals((TSpecimen)obj);
            }
            return false;
        }

        public abstract bool Equals(TSpecimen? other);
    }
}
