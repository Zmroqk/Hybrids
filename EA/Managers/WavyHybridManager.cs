using Loggers;
using Loggers.CSV;
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
    public class WavyHybridManager : IManager<Specimen>
    {
        public TabuSearchWithInitSpecimenManagerFactory TabuSearchFactory { get; set; }
        public SimulatedAnnealingWithInitSpecimenManagerFactory SimulatedAnnealingFactory { get; set; }
        public ISpecimenFactory<Specimen> SpecimenFactory { get; set; } 

        public double StartingTemperature { get; set; }
        public double StartingTemperatureChange { get; set; }
        public double Iterations { get; set; }

        public MetaheuristicType StartingMetaheuristic { get; set; }

        public event EventHandler<WavyHybridRecord> RecordCreated;

        private MetaheuristicType CurrentMetaheuristic { get; set; }
        private double CurrentTemperature { get; set; }

        public WavyHybridManager(TabuSearchWithInitSpecimenManagerFactory tabuSearchFactory
            , SimulatedAnnealingWithInitSpecimenManagerFactory simulatedAnnealingFactory
            , ISpecimenFactory<Specimen> specimenFactory
            , double startingTemperature
            , double startingTemperatureChange
            , double iterations
            , MetaheuristicType startingMetaheuristic
            )
        {
            this.TabuSearchFactory = tabuSearchFactory;
            this.SimulatedAnnealingFactory = simulatedAnnealingFactory;
            this.SpecimenFactory = specimenFactory;
            this.StartingTemperature = startingTemperature;
            this.StartingTemperatureChange = startingTemperatureChange;
            this.Iterations = iterations;
            this.StartingMetaheuristic = startingMetaheuristic;
        }

        private IManager<Specimen> GetCurrentManager(Specimen specimen)
        {
            switch(this.CurrentMetaheuristic)
            {
                case MetaheuristicType.TabuSearch:
                    this.CurrentMetaheuristic = MetaheuristicType.SimulatedAnnealing;
                    return TabuSearchFactory.CreateManager(specimen);
                case MetaheuristicType.SimulatedAnnealing:
                    var manager = this.SimulatedAnnealingFactory.CreateManager(specimen, this.CurrentTemperature);
                    this.CurrentTemperature -= this.StartingTemperatureChange;
                    this.CurrentMetaheuristic = MetaheuristicType.TabuSearch;
                    return manager;
            }
            return null;
        }

        public Specimen FindBestSpecimen()
        {
            this.CurrentMetaheuristic = this.StartingMetaheuristic;
            this.CurrentTemperature = this.StartingTemperature;
            var current = this.SpecimenFactory.CreateSpecimen();
            var best = current;
            for(int i = 0; i < this.Iterations; i++)
            {
                var manager = this.GetCurrentManager(current);
                var specimen = manager.RunManager();
                current = specimen;
                if(specimen.Evaluate() > best.Evaluate())
                {
                    best = specimen;
                }
                var record = new WavyHybridRecord()
                {
                    Iteration = i + 1,
                    BestScore = best.Evaluate(),
                    CurrentScore = current.Evaluate()
                };
                this.RecordCreated?.Invoke(this, record);
                this.CurrentTemperature -= this.StartingTemperatureChange;
            }
            return best;
        }

        public Specimen RunManager()
        {
            return FindBestSpecimen();
        }
    }
}
