using EA.Core;
using Loggers;
using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.DataTTP.Loggers;

namespace Meta.DataTTP
{
    public class LearningManager : LearningManagerBase<Specimen, Record>
    {
        public Data Config { get; set; }

        public LearningManager(Data config
            , IMutator<Specimen> mutator
            , ICrossover<Specimen> crossover
            , ISelector<Specimen> selector
            , ISpecimenFactory<Specimen> specimenFactory
            , uint populationSize
            , ILogger<Record>? logger = null
            , IAdditionalOperations<Specimen> additionalOperations = null
            )
            : base(mutator, crossover, selector, specimenFactory, populationSize, logger, additionalOperations)
        {
            this.Config = config;
        }
    }
}
