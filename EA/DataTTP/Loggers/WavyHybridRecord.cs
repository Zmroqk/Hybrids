using Loggers.CSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Loggers
{
    public class WavyHybridRecord : IRecord
    {
        public int Iteration { get; set; }
        public double BestScore { get; set; }
        public double CurrentScore { get; set; }
    }
}
