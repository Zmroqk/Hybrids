using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;

namespace SA.Core
{
    public interface ISimulatedAnnealing<TSpecimen> :IManager<TSpecimen> where TSpecimen : ISpecimen<TSpecimen>
    {
        INeighborhood<TSpecimen> Neighborhood { get; }
        TSpecimen GetStartingSpecimen();
        TSpecimen RunSimulatedAnnealing();
    }
}
