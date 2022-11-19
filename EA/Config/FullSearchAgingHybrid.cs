using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class FullSearchAgingHybrid : FullSearchEvolutionaryAlgorithm
    {
        public int[] Ages { get; set; }
        public int[] AgeVarieties { get; set; }
        public bool UseLogging { get; set; }
        public string LoggingTemplatePath { get; set; }
    }
}
