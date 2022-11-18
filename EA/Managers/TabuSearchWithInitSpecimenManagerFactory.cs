using Loggers;
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
    public class TabuSearchWithInitSpecimenManagerFactory
    {
        private INeighborhood<Specimen> neighborhood;
        private ILogger<TabuRecord> logger;
        private int iterations;
        private int tabuSize;
        private int neighbourhoodSize;

        public TabuSearchWithInitSpecimenManagerFactory(INeighborhood<Specimen> neighborhood, ILogger<TabuRecord> logger, int iterations, int tabuSize, int neighbourhoodSize)
        {
            this.neighborhood = neighborhood;
            this.logger = logger;
            this.iterations = iterations;
            this.tabuSize = tabuSize;
            this.neighbourhoodSize = neighbourhoodSize;
        }

        public ITabuSearch<Specimen> CreateManager(Specimen specimen)
        {
            var manager = new TabuSearchWithInitSpecimenManager(specimen
                , this.neighborhood
                , this.logger
                , this.iterations
                , this.neighbourhoodSize
                , this.tabuSize
            );
            return manager;
        }
    }
}
