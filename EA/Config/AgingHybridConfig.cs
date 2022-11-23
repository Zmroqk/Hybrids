using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class AgingHybridConfig : LearningConfig
    {
        public int ConfigIndex { get; set; }
        public int Age { get; set; }
        public int AgeVariety { get; set; }
        public bool UseLogging { get; set; }
        public string LoggingTemplatePath { get; set; }
    }
}
