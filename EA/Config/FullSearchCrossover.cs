using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class FullSearchCrossover
    {
        public CrossoverType[] Types { get; set; }
        public double MinProbability { get; set; }
        public double MaxProbability { get; set; }
        public double ProbabilityChange { get; set; } = 0.001d;
        public double[] Probabilities { get; set; }

    }
}
