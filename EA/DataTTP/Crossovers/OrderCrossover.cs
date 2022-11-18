using EA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Crossovers
{
    public class OrderCrossover : ICrossover<Specimen>
    {
        public double Probability { get; set; }
        Random random;
        public OrderCrossover(double probability)
        {
            this.Probability = probability;
            this.random = new Random();
        }

        public IList<Specimen> Crossover(IList<Specimen> specimens)
        {           
            var prop = 1 - this.Probability;
            var newSpecimens = new List<Specimen>();
            for(int i = 0; i < specimens.Count - 1; i++)
            {
                if (prop <= random.NextDouble())
                {
                    var newSpecimen = this.CrossSpecimens(specimens[i], specimens[i + 1]);
                    newSpecimen.IsCrossed = true;
                    newSpecimens.Add(newSpecimen);
                }
                else
                {
                    newSpecimens.Add(specimens[i]);
                }
            }
            newSpecimens.Add(specimens[specimens.Count - 1]);           
            return newSpecimens;
        }

        private Specimen CrossSpecimens(Specimen specimen, Specimen otherSpecimen)
        {
            var newSpecimen = otherSpecimen.Clone();
            var startIndex = random.Next(specimen.Nodes.Count);
            var length = random.Next(specimen.Nodes.Count - startIndex + 1);
            var nodes = specimen.Nodes.GetRange(startIndex, length);
            foreach(var node in nodes)
            {
                newSpecimen.Nodes.Remove(node);
            }
            newSpecimen.Nodes.InsertRange(startIndex > newSpecimen.Nodes.Count ? newSpecimen.Nodes.Count - 1 : startIndex, nodes);
            newSpecimen.Fix();
            return newSpecimen;
        }
    }
}
