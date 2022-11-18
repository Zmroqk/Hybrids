using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class FullSearchTabu
    {
        public MutatorType[] Mutators { get; set; }
        public bool[] UseGreedyKnapsack { get; set; }
        public SpecimenInitializatorConfig[] SpecimenInitializators { get; set; }
        public int MinNeighborhoodSize { get; set; }
        public int MaxNeighborhoodSize { get; set; }
        public int NeighborhoodSizeChange { get; set; } = 1;
        public int MinTabuSize { get; set; }
        public int MaxTabuSize { get; set; }
        public int TabuSizeChange { get; set; } = 1;
        public int MinIterations { get; set; }
        public int MaxIterations { get; set; }
        public int IterationsChange { get; set; } = 1;
    }
}
