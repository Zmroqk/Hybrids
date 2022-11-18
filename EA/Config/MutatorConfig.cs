using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class MutatorConfig
    {
        /// <summary>
        /// Mutator method to be used
        /// </summary>
        public MutatorType Type { get; set; }

        /// <summary>
        /// Mutate probability
        /// </summary>
        public double MutateRatio { get; set; }
    }
}
