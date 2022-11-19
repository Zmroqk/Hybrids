using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA.Core;
using Meta.Core;

namespace Meta.DataTTP.Mutators
{
    public class SwapMutatorBase<TSpecimen> : IMutator<TSpecimen> where TSpecimen : TTPSpecimenBase<TSpecimen>
    {
        public Data Config { get; set; }
        public double MutateRatio { get; set; }
        public double Probability
        {
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

        public SwapMutatorBase(Data config, double mutateRatio)
        {
            this.Config = config;
            this.MutateRatio = mutateRatio;
            this.random = new Random();
        }

        public IList<TSpecimen> MutateAll(IList<TSpecimen> currentPopulation)
        {      
            var newPopulation = new List<TSpecimen>();
            foreach (TSpecimen specimen in currentPopulation)
            {
                var newSpecimen = specimen;
                this.Mutate(newSpecimen);
                newPopulation.Add(newSpecimen);
            }        
            return newPopulation;
        }

        public virtual TSpecimen Mutate(TSpecimen specimen)
        {
            var probability = 1 - this.MutateRatio;
            for (int i = 0; i < specimen.Nodes.Count; i++)
            {
                if (probability <= random.NextDouble())
                {
                    var index2 = random.Next(specimen.Nodes.Count);
                    var swappedNode = specimen.Nodes[i];
                    specimen.Nodes[i] = specimen.Nodes[index2];
                    specimen.Nodes[index2] = swappedNode;
                    specimen.IsMutated = true;
                }
            }
            return specimen;
        }
    }
}
