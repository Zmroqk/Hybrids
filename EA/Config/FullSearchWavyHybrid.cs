using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class FullSearchWavyHybrid : IConfig
    {
        public MutatorType[] MutatorsSA { get; set; }
        public MutatorType[] MutatorsTS { get; set; }
        public bool[] UseGreedyKnapsack { get; set; }
        public SpecimenInitializatorConfig[] SpecimenInitializators { get; set; }
        public int[] NeighbourhoodSizeSA { get; set; }
        public int[] NeighbourhoodSizeTS { get; set; }
        public double[] AnnealingRate { get; set; }
        public int[] StartingTemperature { get; set; }
        public int[] TargetTemperature { get; set; }
        public int[] Iterations { get; set; }
        public int[] StartingTemperatureChange { get; set; }
        public int[] TabuSize { get; set; }
        public int[] HybridIterations { get; set; }
        public string[] Include { get; set; }
        public bool UseAdditionalLogging { get; set; }
        public string AdditionalLoggingPathTemplate { get; set; }
        public bool UseLogging { get; set; }
        public string LoggingPathTemplate { get; set; }
        public MetaheuristicType[] StartingMetaheuristicTypes { get; set; }
    }
}
