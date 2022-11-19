using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA.Core;

namespace Meta.DataTTP.Mutators
{
    public class InverseMutatorBase<TSpecimen> : IMutator<TSpecimen> where TSpecimen : TTPSpecimenBase<TSpecimen>
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
        protected Random random;

        public InverseMutatorBase(Data config, double mutateRatio)
        {
            this.Config = config;
            this.MutateRatio = mutateRatio;
            this.random = new Random();
        }

        public IList<TSpecimen> MutateAll(IList<TSpecimen> currentPopulation)
        {         
            var newPopulation = new List<TSpecimen>();
            foreach(TSpecimen specimen in currentPopulation)
            {
                var newSpecimen = specimen;
                this.Mutate(newSpecimen);
                newPopulation.Add(newSpecimen);
            }
            return newPopulation;
        }

        public TSpecimen Mutate(TSpecimen specimen)
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
