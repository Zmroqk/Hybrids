using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Inititializators
{
    public class RandomSpecimenInitializatorBase<TSpecimen> : ISpecimenInitializator<TSpecimen> where TSpecimen : TTPSpecimenBase<TSpecimen>
    {
        public Data Config { get; set; }
        public double ItemAddProbability { get; set; }
        protected Random random;
        public RandomSpecimenInitializatorBase(Data config, double itemAddProbability)
        {
            this.Config = config;
            this.ItemAddProbability = itemAddProbability;
            this.random = new Random();
        }

        public virtual void Initialize(TSpecimen specimen)
        {
            List<Node> cities = this.Config.Nodes.ToList();
            while(cities.Count > 0)
            {
                var city = cities[random.Next(0, cities.Count)];
                specimen.Nodes.Add(city);
                var probability = 1 - this.ItemAddProbability;
                var items = city.AvailableItems.ToList();
                if (probability <= random.NextDouble() && city.AvailableItems.Count > 0)
                {
                    var index = random.Next(items.Count);
                    specimen.AddItemToKnapsack(items[index]);
                    items.RemoveAt(index);
                }
                cities.Remove(city);
            }
        }
    }
}
