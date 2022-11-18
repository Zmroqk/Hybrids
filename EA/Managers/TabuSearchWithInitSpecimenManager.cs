using Loggers;
using Meta.Core;
using Meta.DataTTP;
using Meta.DataTTP.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;

namespace Meta.Managers
{
    public class TabuSearchWithInitSpecimenManager : TabuSearchManager
    {
        public Specimen Specimen { get; set; }

        public TabuSearchWithInitSpecimenManager(Specimen specimen
            , INeighborhood<Specimen> neighborhood
            , ILogger<TabuRecord>? logger
            , int iterations
            , int neighborhoodSize
            , int tabuSize
            ) : base(null, neighborhood, logger, iterations, neighborhoodSize, tabuSize)
        {
            this.Specimen = specimen;
        }

        public override Specimen GetStartingSpecimen()
        {
            return this.Specimen;
        }
    }
}
