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
        public WavyHybridManager Create(WavyHybridConfig config, out CSVLogger<Specimen, WavyHybridRecord>? logger, string innerPath)
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
                var innerCsvLogger = new CSVLogger<Specimen, WavyHybridRecord>(innerPath);
                innerCsvLogger.RunLogger();
                innerLogger = innerCsvLogger;
                var transformLoggerTabu = new TransformLogger<TabuRecord, WavyHybridRecord>(innerLogger, profile);
                var transformerLoggerAnnealing = new TransformLogger<SimulatedAnnealingRecord, WavyHybridRecord>(innerLogger, profile);
                transformLoggerTabu.Init();
                transformerLoggerAnnealing.Init();
                innerTabuLogger = transformLoggerTabu;
                innerSimulatedAnnealingLogger = transformerLoggerAnnealing;
                logger = innerCsvLogger;
            }
            else
            {
                logger = null;
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
                , config.HybridIterations
                , config.StartingMetaheuristic
            );
        }
    }
}
