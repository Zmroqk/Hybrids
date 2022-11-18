using Loggers.CSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;

namespace Meta.DataTTP.Loggers
{
    public class TabuRecord : IRecord
    {
        public double BestSpecimenScore { get; set; }
        public double AverageSpecimenScore { get; set; }
        public double CurrentSpecimenScore { get; set; }
        public double WorstSpecimenScore { get; set; }
        public int Generation { get; set; }
    }
}
