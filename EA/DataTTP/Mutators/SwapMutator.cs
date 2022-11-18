using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA.Core;

namespace Meta.DataTTP.Mutators
{
    public class SwapMutator : IMutator<Specimen>
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
        Random random;
        public SwapMutator(Data config, double mutateRatio)
        {
            this.Config = config;
            this.MutateRatio = mutateRatio;
            this.random = new Random();
        }

        public IList<Specimen> MutateAll(IList<Specimen> currentPopulation)
        {      
            var newPopulation = new List<Specimen>();
            foreach (Specimen specimen in currentPopulation)
            {
                var newSpecimen = specimen;
                this.Mutate(newSpecimen);
                newPopulation.Add(newSpecimen);
            }        
            return newPopulation;
        }

        public virtual Specimen Mutate(Specimen specimen)
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
