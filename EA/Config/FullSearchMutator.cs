using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class FullSearchMutator
    {
        public MutatorType[] Types { get; set; }

        public double MinMutateRatio { get; set; }
        public double MaxMutateRatio { get; set; }
        public double MutateRatioChange { get; set; } = 0.001;
    }
}
