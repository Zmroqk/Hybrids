using Loggers;
using Meta.Core;
using SA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;
using Meta.DataTTP;
using Meta.DataTTP.Loggers;

namespace Meta.Managers
{
    public class SimulatedAnnealingManager : ISimulatedAnnealing<Specimen>
    {
        public INeighborhood<Specimen> Neighborhood { get; set; }
        public ISpecimenFactory<Specimen> SpecimenFactory { get; set; }
        public ILogger<SimulatedAnnealingRecord> Logger { get; set; }
        public double AnnealingRatio { get; set; }
        public int Iterations { get; set; }
        public int NeighbourhoodSize { get; set; }
        public double StartingTemperature { get; set; }
        public double TargetTemperature { get; set; }

        public SimulatedAnnealingManager(INeighborhood<Specimen> neighborhood
            , ISpecimenFactory<Specimen> specimenFactory
            , ILogger<SimulatedAnnealingRecord> logger
            , double annealingRatio
            , int iterations
            , int neighbourhoodSize
            , double startingTemperature
            , double targetTemperature
            ) {
            this.Neighborhood = neighborhood;
            this.SpecimenFactory = specimenFactory;
            this.Logger = logger;
            this.AnnealingRatio = annealingRatio;
            this.Iterations = iterations;
            this.NeighbourhoodSize = neighbourhoodSize;
            this.StartingTemperature = startingTemperature;
            this.TargetTemperature = targetTemperature;
        }

        public Specimen RunManager()
        {
            var current = this.GetStartingSpecimen();
            var currentScore = current.Evaluate();
            var currentTemperature = this.StartingTemperature;
            var bestScore = currentScore;
            var worstScore = currentScore;
            var best = current;
            var iteration = 0;
            var random = new Random();
            while(iteration < this.Iterations && this.TargetTemperature < currentTemperature)
            {
                var specimens = this.Neighborhood.FindNeighborhood(current, this.NeighbourhoodSize);
                foreach(var specimen in specimens)
                {
                    if (currentScore < specimen.Evaluate() || random.NextDouble() < Math.Exp((specimen.Evaluate() - current.Evaluate()) / currentTemperature))
                    {
                        current = specimen;
                        currentScore = specimen.Evaluate();
                        if (bestScore < currentScore)
                        {
                            best = specimen;
                            bestScore = currentScore;
                        }
                        if (worstScore > currentScore)
                        {
                            worstScore = currentScore;
                        }
                    }
                }
                currentTemperature = currentTemperature * this.AnnealingRatio;
                iteration++;
                var record = new SimulatedAnnealingRecord()
                {
                    Iteration = iteration,
                    CurrentScore = currentScore,
                    BestScore = bestScore,
                    AverageScore = specimens.Average(s => s.Evaluate()),
                    WorstScore = worstScore
                };
                this.Logger?.Log(record);
            }
            return best;
        }

        public virtual Specimen GetStartingSpecimen()
        {
            return this.SpecimenFactory.CreateSpecimen();
        }

        public Specimen RunSimulatedAnnealing()
        {
            return this.RunManager();
        }
    }
}
