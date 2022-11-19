using EA.Core.Selectors;
using EA.Core;
using Meta.Config;
using Meta.Core;
using Meta.DataTTP.AdditionalOperations;
using Meta.DataTTP.Crossovers;
using Meta.DataTTP.Inititializators;
using Meta.DataTTP.Mutators;
using Meta.DataTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabuSearch.Core;
using Meta.DataTTP.Neighborhoods;
using Loggers;
using Meta.DataTTP.Loggers;
using Loggers.TransformLogger;
using Loggers.CSV;

namespace Meta.Managers
{
    public class WavyHybridManagerFactory
    {
        private static int FileIndex = 0;
        private static object Lock = new object();

        public WavyHybridManager Create(WavyHybridConfig config)
        {
            var dataLoader = DataLoader.Instance;
            var data = dataLoader.GetData(config.InputFileName);

            INeighborhood<Specimen> neighbourhoodSA;
            INeighborhood<Specimen> neighbourhoodTS;
            ISpecimenInitializator<Specimen> specimenInitializator;
            ISelector<Specimen> selector;
            ICrossover<Specimen> crossover;
            IMutator<Specimen> knapsackMutator = new KnapsackMutator(data, config.UseGreedyKnapsack);
            if (config.MutatorSA == MutatorType.Swap)
            {
                neighbourhoodSA = new Neighbourhood(new TabuSwapMutator(data), knapsackMutator);
            }
            else
            {
                neighbourhoodSA = new Neighbourhood(new InverseMutator(data, 1), knapsackMutator);
            }
            if (config.MutatorTS == MutatorType.Swap)
            {
                neighbourhoodTS = new Neighbourhood(new TabuSwapMutator(data), knapsackMutator);
            }
            else
            {
                neighbourhoodTS = new Neighbourhood(new InverseMutator(data, 1), knapsackMutator);
            }
            if (config.SpecimenInitializator.Type == SpecimenInitializatorType.Random)
            {
                specimenInitializator = new RandomSpecimenInitializator(data, config.SpecimenInitializator.ItemAddPropability);
            }
            else
            {
                specimenInitializator = new GreedySpecimenInitializator(data, new KnapsackMutator(data, true));
            }
            var specimenFactory = new SpecimenFactory(data, specimenInitializator);
            ILogger<TabuRecord>? innerTabuLogger = null;
            ILogger<SimulatedAnnealingRecord>? innerSimulatedAnnealingLogger = null;
            ILogger<WavyHybridRecord> innerLogger = null;
            if(config.UseAdditionalLogging)
            {
                var profile = MapperProfile.Profile;
                string path;
                lock (Lock)
                {
                    path = string.Format(config.AdditionalLoggingPathTemplate, FileIndex++);
                }
                innerLogger = new CSVLogger<Specimen, WavyHybridRecord>(path);
                innerTabuLogger = new TransformLogger<TabuRecord, WavyHybridRecord>(innerLogger, profile);
                innerSimulatedAnnealingLogger = new TransformLogger<SimulatedAnnealingRecord, WavyHybridRecord>(innerLogger, profile);
            }
            var tabuSearchFactory = new TabuSearchWithInitSpecimenManagerFactory(neighbourhoodTS
                , innerTabuLogger
                , config.Iterations
                , config.TabuSize
                , config.NeighbourhoodSizeTS
            );
            var simulatedAnnealingFactory = new SimulatedAnnealingWithInitSpecimenManagerFactory(neighbourhoodSA
                , innerSimulatedAnnealingLogger
                , config.AnnealingRate
                , config.Iterations
                , config.TargetTemperature
                , config.NeighbourhoodSizeSA
            );

            return new WavyHybridManager(tabuSearchFactory
                , simulatedAnnealingFactory
                , specimenFactory
                , config.StartingTemperature
                , config.StartingTemperatureChange
                , config.Iterations
                , config.StartingMetaheuristic
            );
        }
    }
}
