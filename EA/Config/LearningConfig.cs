using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class LearningConfig : IConfig
    {
        public MutatorConfig Mutator { get; set; }
        public SelectorConfig Selector { get; set; }
        public CrossoverConfig Crossover { get; set; }
        public SpecimenInitializatorConfig SpecimenInitializator { get; set; }
        public int PopulationSize { get; set; }
        public string? TestName { get; set; }
        public string OutputFileName { get; set; }
        public string InputFileName { get; set; }
        public int Epochs { get; set; }
        /// <summary>
        /// Number of times current config will be run
        /// </summary>
        public int RunCount { get; set; }
        /// <summary>
        /// Should this learning config be run on another thread
        /// </summary>
        public bool RunAsTask { get; set; }

        public string[] Include { get; set; }
    }
}
