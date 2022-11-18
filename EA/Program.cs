using EA.Core;
using Loggers.CSV;
using Meta.Core;
using TabuSearch.Core;
using Meta;
using Meta.Config;
using Meta.DataTTP;
using Meta.DataTTP.Inititializators;
using Meta.DataTTP.Loggers;
using Meta.DataTTP.Mutators;
using Meta.DataTTP.Neighborhoods;
using Meta.Managers;
using System.Threading.Tasks;

Console.WriteLine("Select mode: ");
Console.WriteLine("0. EA ");
Console.WriteLine("1. Tabu search ");
Console.WriteLine("2. Random ");
Console.WriteLine("3. Simulated Annealing ");
Console.WriteLine("4. Full search ");
Console.WriteLine("5. Wavy hybrid ");
string mode = Console.ReadLine();
int modeNumber;
if(!int.TryParse(mode, out modeNumber))
{
    modeNumber = 1;
}
if (modeNumber == 0)
{
    var factory = new EAManagerFactory();
    var loader = new LearningConfigLoader<LearningConfig>();
    Console.WriteLine("Path to config: ");
    var path = Console.ReadLine();
    var configs = loader.Load(string.IsNullOrWhiteSpace(path) ? "LearningManager.json" : path);
    foreach(var config in configs)
    {
        var manager = factory.Create(config);
        manager.Init();
        for(int i = 0; i < config.Epochs; i++)
        {
            Console.WriteLine(i);
            manager.NextEpoch();
        }
        Console.WriteLine(manager.Best.Evaluate());
    }
}
else if (modeNumber == 1)
{
    var loader = new LearningConfigLoader<TabuConfig>();
    Console.WriteLine("Path to config: ");
    var path = Console.ReadLine();
    var configs = loader.Load(string.IsNullOrWhiteSpace(path) ? "TabuConfig.json" : path);

    foreach (var config in configs)
    {
        var dataLoader = new DataLoader();
        var data = dataLoader.Load(config.InputFileName);
        ISpecimenInitializator<Specimen> initializator;
        if (config.SpecimenInitializator.Type == SpecimenInitializatorType.Greedy)
        {
            initializator = new GreedySpecimenInitializator(data, new KnapsackMutator(data, true));
        }
        else
        {
            initializator = new RandomSpecimenInitializator(data, config.SpecimenInitializator.ItemAddPropability);
        }
        var factory = new SpecimenFactory(data, initializator);
        IMutator<Specimen> mutator;
        if (config.Mutator == MutatorType.Swap)
        {
            mutator = new TabuSwapMutator(data);
        }
        else
        {
            mutator = new InverseMutator(data, 1);
        }
        var knapsackMutator = new KnapsackMutator(data, config.GreedyKnapsackMutator);
        var neighbourhood = new Neighbourhood(mutator, knapsackMutator);
        var logger = new CSVLogger<Specimen, TabuRecord>(config.OutputFileName);
        logger.RunLogger();

        var tabuSearch = new TabuSearchManager(factory, neighbourhood, logger, config.Iterations, config.NeighborhoodSize, config.TabuSize);
        Console.WriteLine(config.TestName);
        tabuSearch.RunTabuSearch();

        logger.Wait();
        logger.Dispose();
    }
}
else if(modeNumber == 2)
{
    var dataLoader = new DataLoader();
    var data = dataLoader.Load("ai-lab1-ttp_data/medium_0.ttp");
    ISpecimenInitializator<Specimen> initializator = new RandomSpecimenInitializator(data, 0.3);
    var logger = new CSVLogger<Specimen, RandomRecord>("Random.csv");
    logger.RunLogger();
    for (int i = 0; i < 10000; i++)
    {
        var specimen = new Specimen(data, initializator);
        specimen.Init();
        logger.Log(new RandomRecord()
        {
            Value = specimen.Evaluate()
        });
    }
    logger.Wait();
    logger.Dispose();
}
else if (modeNumber == 3)
{
    var loader = new LearningConfigLoader<SimulatedAnnealingConfig>();
    Console.WriteLine("Path to config: ");
    var path = Console.ReadLine();
    var configs = loader.Load(string.IsNullOrWhiteSpace(path) ? "SimulatedAnnealingConfig.json" : path);

    foreach (var config in configs)
    {
        var dataLoader = new DataLoader();
        var data = dataLoader.Load(config.InputFileName);
        ISpecimenInitializator<Specimen> initializator;
        if (config.SpecimenInitializator.Type == SpecimenInitializatorType.Greedy)
        {
            initializator = new GreedySpecimenInitializator(data, new KnapsackMutator(data, true));
        }
        else
        {
            initializator = new RandomSpecimenInitializator(data, config.SpecimenInitializator.ItemAddPropability);
        }
        var factory = new SpecimenFactory(data, initializator);
        IMutator<Specimen> mutator;
        if (config.Mutator == MutatorType.Swap)
        {
            mutator = new TabuSwapMutator(data);
        }
        else
        {
            mutator = new InverseMutator(data, 1);
        }
        var knapsackMutator = new KnapsackMutator(data, config.GreedyKnapsackMutator);
        var neighbourhood = new Neighbourhood(mutator, knapsackMutator);
        var logger = new CSVLogger<Specimen, SimulatedAnnealingRecord>(config.OutputFileName);
        logger.RunLogger();

        var simulatedAnnealing = new SimulatedAnnealingManager(neighbourhood
            , factory
            , logger
            , config.AnnealingRate
            , config.Iterations
            , config.NeighborhoodSize
            , config.StartingTemperature
            , config.TargetTemperature
            );
        Console.WriteLine(config.TestName);
        simulatedAnnealing.RunSimulatedAnnealing();

        logger.Wait();
        logger.Dispose();
    }
}
else if(modeNumber == 4)
{
    var loader = new LearningConfigLoader<FullSearchConfig>();
    Console.WriteLine("Path to config: ");
    var path = Console.ReadLine();
    var configs = loader.Load(string.IsNullOrWhiteSpace(path) ? "FullSearchConfig.json" : path);
    foreach(var config in configs)
    {
        var manager = new FullSearchParamsManager(config, config.Threads);
        manager.Run();
        manager.Wait();
        manager.Dispose();
    }   
}
else if(modeNumber == 5)
{
    var loader = new LearningConfigLoader<WavyHybridConfig>();
    Console.WriteLine("Path to config: ");
    var path = Console.ReadLine();
    var configs = loader.Load(string.IsNullOrWhiteSpace(path) ? "WavyHybridConfig.json" : path);
    var factory = new WavyHybridManagerFactory();
    foreach(var config in configs)
    {
        Parallel.For(0, config.RunCount, (i, state) =>
        {
            CSVLogger<Specimen, WavyHybridRecord>? logger = null;
            string path;
            if (config.UseLogging)
            {
                path = string.Format(config.LoggingPathTemplate, i);
                logger = new CSVLogger<Specimen, WavyHybridRecord>(path);
            }
            //local function
            void Manager_RecordCreated(object sender, WavyHybridRecord e)
            {
                logger?.Log(e);
            }
            //
            logger?.RunLogger();
            var manager = factory.Create(config);
            manager.RecordCreated += Manager_RecordCreated;
            var specimen = manager.RunManager();

            logger?.Wait();
            logger?.Dispose();
            manager.RecordCreated -= Manager_RecordCreated;
        });
    }
}

