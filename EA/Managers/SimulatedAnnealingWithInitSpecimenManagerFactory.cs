using Loggers;
using Meta.DataTTP;
using Meta.DataTTP.Loggers;
using SA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;

namespace Meta.Managers
{
    public class SimulatedAnnealingWithInitSpecimenManagerFactory
    {
        INeighborhood<Specimen> neighborhood;
        ILogger<SimulatedAnnealingRecord> logger;
        double annealingRatio;
        int irerations;
        int neighbourhoodSize;
        double targetTemperature;

        public SimulatedAnnealingWithInitSpecimenManagerFactory(INeighborhood<Specimen> neighborhood, ILogger<SimulatedAnnealingRecord> logger, double annealingRatio, int irerations, double targetTemperature, int neighbourhoodSize)
        {
            this.neighborhood = neighborhood;
            this.logger = logger;
            this.annealingRatio = annealingRatio;
            this.irerations = irerations;
            this.targetTemperature = targetTemperature;
            this.neighbourhoodSize = neighbourhoodSize;
        }

        public ISimulatedAnnealing<Specimen> CreateManager(Specimen specimen, double startingTemperature)
        {
            var manager = new SimulatedAnnealingWithInitSpecimenManager(this.neighborhood
                , specimen
                , this.logger
                , this.annealingRatio
                , this.irerations
                , this.neighbourhoodSize
                , startingTemperature
                , this.targetTemperature);
            return manager;
        }
    }
}
