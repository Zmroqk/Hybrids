using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class FullSearchEvolutionaryAlgorithm
    {
        public FullSearchMutator Mutator { get; set; }
        public FullSearchSelector Selector { get; set; }
        public FullSearchCrossover Crossover { get; set; }
        public SpecimenInitializatorConfig[] SpecimenInitializators { get; set; }
        public int MinPopulationSize { get; set; }
        public int MaxPopulationSize { get; set; }
        public int PopulationSizeChange { get; set; } = 1;
        public int MinEpochs { get; set; }
        public int MaxEpochs { get; set; }
        public int EpochsChange { get; set; } = 1;
    }
}
