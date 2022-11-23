using Loggers.CSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Core
{
    public interface IEARecord : IRecord
    {
        public double BestScore { get; set; }
        public double AverageScore { get; set; }
        public double WorstScore { get; set; }
    }
}
