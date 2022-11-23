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
    public class EAManager : EAManagerBase<Specimen, EARecord>
    {
        public Data Config { get; set; }

        public EAManager(Data config
            , IMutator<Specimen> mutator
            , ICrossover<Specimen> crossover
            , ISelector<Specimen> selector
            , ISpecimenFactory<Specimen> specimenFactory
            , uint populationSize
            , int epochs
            , ILogger<EARecord>? logger = null
            , IAdditionalOperations<Specimen> additionalOperations = null
            )
            : base(mutator, crossover, selector, specimenFactory, populationSize, epochs, logger, additionalOperations)
        {
            this.Config = config;
        }
    }
}
