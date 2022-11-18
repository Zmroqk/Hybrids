using Loggers;
using Loggers.CSV;
using Meta.Core;

namespace TabuSearch.Core
{
    public interface ITabuSearch<TSpecimen> : IManager<TSpecimen> where TSpecimen : ISpecimen<TSpecimen>
    {
        INeighborhood<TSpecimen> Neighborhood { get; }
        TSpecimen GetStartingSpecimen();
        TSpecimen RunTabuSearch();
        IEnumerable<TSpecimen> TabuList();
    }
}
