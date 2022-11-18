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
    public class SimulatedAnnealingWithInitSpecimenManager : SimulatedAnnealingManager
    {
        public Specimen Specimen { get; set; }
        public SimulatedAnnealingWithInitSpecimenManager(INeighborhood<Specimen> neighborhood
            , Specimen specimen
            , ILogger<SimulatedAnnealingRecord> logger
            , double annealingRatio
            , int iterations
            , int neighbourhoodSize
            , double startingTemperature
            , double targetTemperature
            ) : base(neighborhood
                , null
                , logger
                , annealingRatio
                , iterations
                , neighbourhoodSize
                , startingTemperature
                , targetTemperature
                )
        {
            this.Specimen = specimen;
        }

        public override Specimen GetStartingSpecimen()
        {
            return this.Specimen;
        }
    }
}
