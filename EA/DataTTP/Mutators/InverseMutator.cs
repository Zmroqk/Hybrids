using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA.Core;

namespace Meta.DataTTP.Mutators
{
    public class InverseMutator : IMutator<Specimen>
    {
        public Data Config { get; set; }
        public double MutateRatio { get; set; }
        public double Probability { 
            get
            {
                return this.MutateRatio;
            } 
            set
            {
                this.MutateRatio = value;
            }
        }
        Random random;
        public InverseMutator(Data config, double mutateRatio)
        {
            this.Config = config;
            this.MutateRatio = mutateRatio;
            this.random = new Random();
        }

        public IList<Specimen> MutateAll(IList<Specimen> currentPopulation)
        {         
            var newPopulation = new List<Specimen>();
            foreach(Specimen specimen in currentPopulation)
            {
                var newSpecimen = specimen;
                this.Mutate(newSpecimen);
                newPopulation.Add(newSpecimen);
            }
            return newPopulation;
        }

        public Specimen Mutate(Specimen specimen)
        {
            var probability = 1 - this.MutateRatio;
            if (probability <= random.NextDouble())
            {
                var startIndex = random.Next(specimen.Nodes.Count);
                var length = random.Next(specimen.Nodes.Count - startIndex);
                var swappedNodes = specimen.Nodes.GetRange(startIndex, length);
                swappedNodes.Reverse();
                specimen.Nodes.RemoveRange(startIndex, length);
                specimen.Nodes.InsertRange(startIndex, swappedNodes);
                specimen.IsMutated = true;
            }
            return specimen;
        }
    }
}
