using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class TabuConfig : IConfig
    {
        public MutatorType Mutator { get; set; }
        public bool GreedyKnapsackMutator { get; set; }
        public SpecimenInitializatorConfig SpecimenInitializator { get; set; }
        public int NeighborhoodSize { get; set; }
        public int TabuSize { get; set; }
        public string? TestName { get; set; }
        public string OutputFileName { get; set; }
        public string InputFileName { get; set; }
        public int Iterations { get; set; }
        /// <summary>
        /// Number of times current config will be run
        /// </summary>
        public int RunCount { get; set; }

        public string[] Include { get; set; }
    }
}
