using EA.Core;
using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;

namespace Meta.DataTTP.AdditionalOperations
{
    public class AdditionalOperationsWithAgeHandler : IAdditionalOperations<SpecimenWithAge>
    {
        public int AgeForNewSpecimen { get; set; }
        public int AgeVariety { get; set; }
        public IMutator<SpecimenWithAge> KnapsackMutator { get; set; }
        public INeighborhood<SpecimenWithAge>? Neighborhood { get; set; }
        public ISpecimenFactory<SpecimenWithAge>? SpecimenFactory { get; set; }
        private Random random;

        public AdditionalOperationsWithAgeHandler(int ageForNewSpecimen
            , int ageVariety
            , IMutator<SpecimenWithAge> knapsackMutator
            , INeighborhood<SpecimenWithAge>? neighborhood = null
            , ISpecimenFactory<SpecimenWithAge>? specimenFactory = null)
        {
            this.AgeForNewSpecimen = ageForNewSpecimen;
            this.AgeVariety = ageVariety;
            this.KnapsackMutator = knapsackMutator;
            this.Neighborhood = neighborhood;
            this.SpecimenFactory = specimenFactory;
            this.random = new Random();
        }

        public IList<SpecimenWithAge> AfterCrossover(IList<SpecimenWithAge> currentPopulation)
        {
            foreach (var specimen in currentPopulation)
            {
                if (specimen.IsCrossed)
                {
                    specimen.Age = this.random.Next(this.AgeForNewSpecimen - this.AgeVariety, this.AgeForNewSpecimen + this.AgeVariety);
                }
            }
            return currentPopulation;
        }

        public IList<SpecimenWithAge> AfterMutation(IList<SpecimenWithAge> currentPopulation)
        {
            for (int i = 0; i < currentPopulation.Count; i++)
            {
                if (--currentPopulation[i].Age == 0)
                {
                    if (this.Neighborhood != null)
                    {
                        var newSpecimen = this.Neighborhood.FindNeighborhood(currentPopulation[i], 1);
                        currentPopulation[i] = newSpecimen.First();
                    }
                    else if (this.SpecimenFactory != null)
                    {
                        currentPopulation[i] = this.SpecimenFactory.CreateSpecimen();
                    }
                    currentPopulation[i].Age = this.random.Next(this.AgeForNewSpecimen - this.AgeVariety, this.AgeForNewSpecimen + this.AgeVariety);
                    currentPopulation[i].IsMutated = false;
                    currentPopulation[i].IsCrossed = false;
                }
            }
            foreach (var specimen in currentPopulation)
            {
                if (specimen.IsModified)
                {
                    this.KnapsackMutator.Mutate(specimen);
                }
            }
            return currentPopulation;
        }

        public IList<SpecimenWithAge> AfterSelect(IList<SpecimenWithAge> currentPopulation)
        {
            return currentPopulation;
        }

        public IList<SpecimenWithAge> BeforeCrossover(IList<SpecimenWithAge> currentPopulation)
        {
            return currentPopulation;
        }

        public IList<SpecimenWithAge> BeforeMutation(IList<SpecimenWithAge> currentPopulation)
        {
            return currentPopulation;
        }

        public IList<SpecimenWithAge> BeforeSelect(IList<SpecimenWithAge> currentPopulation)
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
