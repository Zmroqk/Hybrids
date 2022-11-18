using EA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.AdditionalOperations
{
    public class AdditionalOperationsHandler : IAdditionalOperations<Specimen>
    {
        public IMutator<Specimen> KnapsackMutator { get; set; }

        public AdditionalOperationsHandler(IMutator<Specimen> knapsackMutator)
        {
            this.KnapsackMutator = knapsackMutator;
        }

        public IList<Specimen> AfterCrossover(IList<Specimen> currentPopulation)
        {
            return currentPopulation;
        }

        public IList<Specimen> AfterMutation(IList<Specimen> currentPopulation)
        {
            foreach (var specimen in currentPopulation)
            {
                if (specimen.IsModified)
                {
                    this.KnapsackMutator.Mutate(specimen);
                }
            }
            return currentPopulation;
        }

        public IList<Specimen> AfterSelect(IList<Specimen> currentPopulation)
        {
            return currentPopulation;
        }

        public IList<Specimen> BeforeCrossover(IList<Specimen> currentPopulation)
        {
            return currentPopulation;
        }

        public IList<Specimen> BeforeMutation(IList<Specimen> currentPopulation)
        {
            return currentPopulation;
        }

        public IList<Specimen> BeforeSelect(IList<Specimen> currentPopulation)
        {
            foreach (var specimen in currentPopulation)
            {
                specimen.IsMutated = false;
                specimen.IsCrossed = false;
            }
            return currentPopulation;
        }
    }
}
