using EA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;

namespace Meta.DataTTP.Neighborhoods
{
    public class Neighbourhood : INeighborhood<Specimen>
    {
        public IMutator<Specimen> Mutator { get; set; }
        public IMutator<Specimen> KnapsackMutator { get; set; }
        public Neighbourhood(IMutator<Specimen> mutator, IMutator<Specimen> knapsackMutator)
        {
            this.Mutator = mutator;
            this.KnapsackMutator = knapsackMutator;
        }

        public IEnumerable<Specimen> FindNeighborhood(Specimen specimen, int size)
        {
            var neighborhoods = new List<Specimen>();
            for(int i = 0; i < size; i++)
            {
                var newSpecimen = this.Mutator.Mutate(specimen.Clone());
                this.KnapsackMutator.Mutate(newSpecimen);
                neighborhoods.Add(newSpecimen);
            }
            return neighborhoods;
        }
    }
}
