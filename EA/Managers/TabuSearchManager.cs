using Loggers;
using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.DataTTP;
using Meta.DataTTP.Loggers;
using TabuSearch.Core;

namespace Meta.Managers
{
    public class TabuSearchManager : ITabuSearch<Specimen>
    {
        public ISpecimenFactory<Specimen> SpecimenFactory { get; set; }

        public int Iterations { get; set; }
        public int NeighborhoodSize { get; set; }
        public int TabuSize { get; set; }

        public INeighborhood<Specimen> Neighborhood { get; set; }

        private List<Specimen> Tabu { get; set; }
        private HashSet<Specimen> TabuHash { get; set; }

        public ILogger<TabuRecord>? Logger { get; set; }

        public TabuSearchManager(ISpecimenFactory<Specimen> specimenFactory
            , INeighborhood<Specimen> neighborhood
            , ILogger<TabuRecord>? logger
            , int iterations
            , int neighborhoodSize
            , int tabuSize
            )
        {
            this.SpecimenFactory = specimenFactory;
            this.Neighborhood = neighborhood;
            this.Logger = logger;
            this.Iterations = iterations;
            this.NeighborhoodSize = neighborhoodSize;
            this.TabuSize = tabuSize;
            this.Tabu = new List<Specimen>();
            this.TabuHash = new HashSet<Specimen>();
        }

        public virtual Specimen RunManager()
        {
            var specimen = this.GetStartingSpecimen();
            var bestSpecimen = specimen;
            var bestScore = bestSpecimen.Evaluate();
            var iteration = 0;
            while (iteration < this.Iterations)
            {
                var neighborhoods = this.Neighborhood.FindNeighborhood(specimen, this.NeighborhoodSize);
                var filteredNeighborhoods = neighborhoods.Where(n => !this.TabuList().Contains(n)).ToList();
                var bestNeighborhood = filteredNeighborhoods.MaxBy(n => n.Evaluate());
                var worstNeighborhood = filteredNeighborhoods.MinBy(n => n.Evaluate());
                if (bestNeighborhood != null)
                {
                    var bestNeighborhoodScore = bestNeighborhood.Evaluate();
                    var worstNeighborhoodScore = worstNeighborhood.Evaluate();
                    if (bestNeighborhoodScore > bestScore)
                    {
                        bestSpecimen = bestNeighborhood;
                        bestScore = bestNeighborhoodScore;
                    }
                    specimen = bestNeighborhood;
                    var record = new TabuRecord()
                    {
                        Generation = iteration,
                        BestSpecimenScore = bestScore,
                        CurrentSpecimenScore = specimen.Evaluate(),
                        AverageSpecimenScore = neighborhoods.Average(n => n.Evaluate()),
                        WorstSpecimenScore = worstNeighborhoodScore
                    };
                    this.Logger?.Log(record);
                    this.Tabu.Add(bestNeighborhood);
                    this.TabuHash.Add(bestNeighborhood);
                }
                if (this.Tabu.Count > this.TabuSize)
                {
                    this.TabuHash.Remove(this.Tabu.First());
                    this.Tabu.RemoveAt(0);
                }
                iteration++;
            }
            return bestSpecimen;
        }

        public IEnumerable<Specimen> TabuList()
        {
            return this.TabuHash;
        }

        public virtual Specimen GetStartingSpecimen()
        {
            return this.SpecimenFactory.CreateSpecimen();
        }

        public Specimen RunTabuSearch()
        {
            return this.RunManager();
        }
    }
}
