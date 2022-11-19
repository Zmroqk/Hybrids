using EA.Core.Selectors;
using EA.Core;
using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.Config;
using Meta.DataTTP;
using Meta.DataTTP.AdditionalOperations;
using Meta.DataTTP.Crossovers;
using Meta.DataTTP.Inititializators;
using Meta.DataTTP.Mutators;
using Loggers;
using Meta.DataTTP.Loggers;

namespace Meta.Managers
{
    public class AgingHybridManagerFactory
    {
        public AgingHybridManager Create(AgingHybridConfig config, ILogger<Record>? logger = null)
        {
            var dataLoader = DataLoader.Instance;
            var data = dataLoader.GetData(config.InputFileName);

            IMutator<SpecimenWithAge> mutator;
            ISpecimenInitializator<SpecimenWithAge> specimenInitializator;
            ISelector<SpecimenWithAge> selector;
            ICrossover<SpecimenWithAge> crossover;

            if (config.Mutator.Type == MutatorType.Swap)
            {
                mutator = new SwapMutatorBase<SpecimenWithAge>(data, config.Mutator.MutateRatio);
            }
            else
            {
                mutator = new InverseMutatorBase<SpecimenWithAge>(data, config.Mutator.MutateRatio);
            }

            if (config.SpecimenInitializator.Type == SpecimenInitializatorType.Greedy)
            {
                specimenInitializator = new GreedySpecimenInitializatorWithAge(data
                    , new KnapsackMutatorBase<SpecimenWithAge>(data, true)
                    , config.Age
                    , config.AgeVariety
                    );
            }
            else
            {
                specimenInitializator = new RandomSpecimenWithAgeInitializator(data
                    , 0.3d
                    , config.Age
                    , config.AgeVariety
                    );
            }

            if (config.Selector.Type == SelectionType.Roulette)
            {
                selector = new RouletteSelection<SpecimenWithAge>(config.Selector.IsMinimalizing);
            }
            else
            {
                selector = new TournamentSelection<SpecimenWithAge>(config.Selector.SpecimenCount, config.Selector.IsMinimalizing);
            }

            if (config.Crossover.Type == CrossoverType.Order)
            {
                crossover = new OrderCrossoverBase<SpecimenWithAge>(config.Crossover.Probability);
            }
            else
            {
                crossover = new PartiallyMatchedCrossoverBase<SpecimenWithAge>(config.Crossover.Probability);
            }

            var additionalOperations = new AdditionalOperationsWithAgeHandler(config.Age
                , config.AgeVariety
                , new KnapsackMutatorBase<SpecimenWithAge>(data, true)
                );
            var specimenFactory = new SpecimenWithAgeFactory(data, specimenInitializator);

            return new AgingHybridManager(mutator
                , crossover
                , selector
                , specimenFactory
                , (uint)config.PopulationSize
                , config.Epochs
                , logger
                , additionalOperations
                );
        }
    }
}
