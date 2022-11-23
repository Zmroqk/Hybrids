using EA.Core;
using Loggers;
using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.DataTTP.Loggers;
using Meta.DataTTP;

namespace Meta.Managers
{
    public class AgingHybridManager : EAManagerBase<SpecimenWithAge, EARecord>
    {
        public AgingHybridManager(IMutator<SpecimenWithAge> mutator
            , ICrossover<SpecimenWithAge> crossover
            , ISelector<SpecimenWithAge> selector
            , ISpecimenFactory<SpecimenWithAge> specimenFactory
            , uint populationSize
            , int epochs
            , ILogger<EARecord>? logger = null
            , IAdditionalOperations<SpecimenWithAge> additionalOperations = null
            )
            : base(mutator, crossover, selector, specimenFactory, populationSize, epochs, logger, additionalOperations)
        {
        }
    }
}
