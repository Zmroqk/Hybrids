using EA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Loggers
{
    public class EARecord : IEARecord
    {
        public double BestScore { get; set; }
        public double AverageScore { get; set; }
        public double WorstScore { get; set; }
    }
}
