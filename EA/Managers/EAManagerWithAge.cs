using EA.Core;
using Loggers;
using Meta.Core;
using Meta.DataTTP;
using Meta.DataTTP.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Managers
{
    public class EAManagerWithAge : EAManagerBase<SpecimenWithAge, Record>
    {
        public EAManagerWithAge(IMutator<SpecimenWithAge> mutator
            , ICrossover<SpecimenWithAge> crossover
            , ISelector<SpecimenWithAge> selector
            , ISpecimenFactory<SpecimenWithAge> specimenFactory
            , uint populationSize
            , int epochs
            , ILogger<Record>? logger = null
            , IAdditionalOperations<SpecimenWithAge> additionalOperations = null
            ) : base(mutator, crossover, selector, specimenFactory, populationSize, epochs, logger, additionalOperations)
        {
        }

        
    }
}
