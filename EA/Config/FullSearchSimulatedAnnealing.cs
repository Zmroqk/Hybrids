using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class FullSearchSimulatedAnnealing
    {
        public MutatorType[] Mutators { get; set; }
        public bool[] UseGreedyKnapsack { get; set; }
        public SpecimenInitializatorConfig[] SpecimenInitializators { get; set; }
        public int MinNeighborhoodSize { get; set; }
        public int MaxNeighborhoodSize { get; set; }
        public int NeighborhoodSizeChange { get; set; } = 1;
        public double MinAnnealingRate { get; set; }
        public double MaxAnnealingRate { get; set; }
        public double AnnealingRateChange { get; set; } = 0.001;
        public double MinStartingTemperature { get; set; }
        public double MaxStartingTemperature { get; set; }
        public double StartingTemperatureChange { get; set; } = 1;
        public double MinTargetTemperature { get; set; }
        public double MaxTargetTemperature { get; set; }
        public double TargetTemperatureChange { get; set; } = 1;
        public int MinIterations { get; set; }
        public int MaxIterations { get; set; }
        public int IterationsChange { get; set; } = 1;
    }
}
