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

namespace Meta.Managers
{
    public class EAManagerFactory
    {
        public LearningManager Create(LearningConfig config)
        {
            var dataLoader = new DataLoader();
            var data = dataLoader.Load(config.InputFileName);
            if (data == null)
            {
                Environment.Exit(-1);
            }

            IMutator<Specimen> mutator;
            ISpecimenInitializator<Specimen> specimenInitializator;
            ISelector<Specimen> selector;
            ICrossover<Specimen> crossover;

            if (config.Mutator.Type == MutatorType.Swap)
            {
                mutator = new SwapMutator(data, config.Mutator.MutateRatio);
            }
            else
            {
                mutator = new InverseMutator(data, config.Mutator.MutateRatio);
            }

            if (config.SpecimenInitializator.Type == SpecimenInitializatorType.Random)
            {
                specimenInitializator = new RandomSpecimenInitializator(data, config.SpecimenInitializator.ItemAddPropability);
            }
            else
            {
                specimenInitializator = new GreedySpecimenInitializator(data, new KnapsackMutator(data, true));
            }

            if (config.Selector.Type == SelectionType.Roulette)
            {
                selector = new RouletteSelection<Specimen>(config.Selector.IsMinimalizing);
            }
            else
            {
                selector = new TournamentSelection<Specimen>(config.Selector.SpecimenCount, config.Selector.IsMinimalizing);
            }

            if (config.Crossover.Type == CrossoverType.Order)
            {
                crossover = new OrderCrossover(config.Crossover.Probability);
            }
            else
            {
                crossover = new PartiallyMatchedCrossover(config.Crossover.Probability);
            }

            var additionalOperations = new AdditionalOperationsHandler(new KnapsackMutator(data, true));
            var specimenFactory = new SpecimenFactory(data, specimenInitializator);

            return new LearningManager(data
                , mutator
                , crossover
                , selector
                , specimenFactory
                , (uint)config.PopulationSize
                , null
                , additionalOperations
                );
        }
    }
}
