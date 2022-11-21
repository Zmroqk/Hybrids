using EA.Core;
using EA.Core.Selectors;
using Loggers;
using Loggers.CSV;
using Meta.Core;
using Meta.Config;
using Meta.DataTTP;
using Meta.DataTTP.AdditionalOperations;
using Meta.DataTTP.Crossovers;
using Meta.DataTTP.Inititializators;
using Meta.DataTTP.Loggers;
using Meta.DataTTP.Mutators;
using Meta.DataTTP.Neighborhoods;

namespace Meta.Managers
{
    public class FullSearchParamsManager : IDisposable
    {
        FullSearchConfig FullSearchConfig { get; set; }
        public int MaxThreads { get; set; }

        private Task managerTask;
        private CancellationTokenSource cancellationToken;
        private List<(Task<List<Specimen>> task, IConfig config)> currentTasks;
        private ILogger<FullSearchRecord> logger;
        private IEnumerator<IConfig> Configs;
        private int rowsLogged;
        private int fileIndex;
        private Dictionary<string, int> additionalLoggingIndexes;
        private object additionalLoggingLock;
        private Dictionary<string, Data> LoadedData { get; set; }
        public FullSearchParamsManager(FullSearchConfig config, int maxThreads)
        {
            this.MaxThreads = maxThreads;
            this.FullSearchConfig = config;
            this.currentTasks = new List<(Task<List<Specimen>>, IConfig)>();
            this.rowsLogged = 0;
            this.fileIndex = 0;
            this.additionalLoggingLock = new object();
            this.LoadedData = new Dictionary<string, Data>();
            this.additionalLoggingIndexes = new Dictionary<string, int>();
        }

        public void Run()
        {
            this.Configs = this.GetConfigsEnumerator();
            this.cancellationToken = new CancellationTokenSource();
            this.managerTask = new Task(() =>
            {
                this.ManagerLoop();
            }, this.cancellationToken.Token);
            this.managerTask.Start();
        }

        private void ManagerLoop()
        {
            this.Configs.MoveNext();
            while (!this.cancellationToken.IsCancellationRequested || this.currentTasks.Count != 0)
            {
                for(int i = 0; i < this.currentTasks.Count; i++)
                {
                    if (this.currentTasks[i].task.IsCompleted)
                    {
                        this.LogData(this.currentTasks[i].task.Result, this.currentTasks[i].config);
                        this.currentTasks.RemoveAt(i);
                        i--;
                    }
                }
                for(int i = this.MaxThreads - this.currentTasks.Count; i > 0; i--)
                {                    
                    var config = this.Configs.Current;
                    var canMoveNext = this.Configs.MoveNext();
                    if (!canMoveNext)
                    {
                        this.cancellationToken.Cancel();
                    }
                    else {
                        switch (config)
                        {
                            case AgingHybridConfig:
                                this.currentTasks.Add((Task.Run(() => this.RunAgingHybrid((AgingHybridConfig)config)), config));
                                break;
                            case LearningConfig:
                                this.currentTasks.Add((Task.Run(() => this.RunEvolutionaryAlgorithm((LearningConfig)config)), config));
                                break;
                            case TabuConfig:
                                this.currentTasks.Add((Task.Run(() => this.RunTabuSearch((TabuConfig)config)), config));
                                break;
                            case SimulatedAnnealingConfig:
                                this.currentTasks.Add((Task.Run(() => this.RunSimulatedAnnealing((SimulatedAnnealingConfig)config)), config));
                                break;
                            case WavyHybridConfig:
                                this.currentTasks.Add((Task.Run(() => this.RunWavyHybrid((WavyHybridConfig)config)), config));
                                break;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }

        private double CalculateStandardError(List<Specimen> population)
        {
            var avg = population.Average(x => x.Evaluate());
            return Math.Sqrt(population.Sum(x => Math.Pow(x.Evaluate() - avg, 2))/population.Count)/Math.Sqrt(population.Count);
        }

        private void LogData(List<Specimen> results, IConfig config)
        {
            FullSearchRecord record;
            switch (config)
            {
                case AgingHybridConfig:
                case LearningConfig:
                    var learningConfig = (LearningConfig)config;
                    if (this.logger == null || this.rowsLogged == 1000000)
                    {
                        this.RecreateLogger(learningConfig.OutputFileName);
                    }
                    record = new FullSearchRecord()
                    {
                        BestScore = results.Max(x => x.Evaluate()),
                        WorstScore = results.Min(x => x.Evaluate()),
                        AverageScore = results.Average(x => x.Evaluate()),
                        StandardError = this.CalculateStandardError(results),
                        PopulationSize = learningConfig.PopulationSize,
                        CrossoverProbability = learningConfig.Crossover.Probability,
                        MutationProbability = learningConfig.Mutator.MutateRatio,
                        SpecimenCount = learningConfig.Selector.SpecimenCount,
                        CrossoverType = learningConfig.Crossover.Type.ToString(),
                        MutatorTypeEA = learningConfig.Mutator.Type.ToString(),
                        SelectorType = learningConfig.Selector.Type.ToString(),
                        Epochs = learningConfig.Epochs,
                        Metaheuristic = "EvolutionaryAlgorithm",
                        FileName = learningConfig.InputFileName,
                        GreeedyKnapsack = true
                    };
                    if(learningConfig is AgingHybridConfig)
                    {
                        var agingHybridConfig = (AgingHybridConfig)learningConfig;
                        record.Age = agingHybridConfig.Age;
                        record.AgeVariety = agingHybridConfig.AgeVariety;
                    }
                    break;
                case TabuConfig:
                    var tabuConfig = (TabuConfig)config;
                    if (this.logger == null || this.rowsLogged == 1000000)
                    {
                        this.RecreateLogger(tabuConfig.OutputFileName);
                    }
                    record = new FullSearchRecord()
                    {
                        BestScore = results.Max(x => x.Evaluate()),
                        WorstScore = results.Min(x => x.Evaluate()),
                        AverageScore = results.Average(x => x.Evaluate()),
                        StandardError = this.CalculateStandardError(results),
                        Iterations = tabuConfig.Iterations,
                        GreeedyKnapsack = tabuConfig.GreedyKnapsackMutator,
                        TabuSize = tabuConfig.TabuSize,
                        MutatorTypeTS = tabuConfig.Mutator.ToString(),
                        NeighborSizeTS = tabuConfig.NeighborhoodSize,   
                        Metaheuristic = "Tabu",
                        FileName = tabuConfig.InputFileName,
                    };
                    break;
                case SimulatedAnnealingConfig:
                    var simulatedAnnealingConfig = (SimulatedAnnealingConfig)config;
                    if (this.logger == null || this.rowsLogged == 1000000)
                    {
                        this.RecreateLogger(simulatedAnnealingConfig.OutputFileName);
                    }
                    record = new FullSearchRecord()
                    {
                        BestScore = results.Max(x => x.Evaluate()),
                        WorstScore = results.Min(x => x.Evaluate()),
                        AverageScore = results.Average(x => x.Evaluate()),
                        StandardError = this.CalculateStandardError(results),
                        Iterations = simulatedAnnealingConfig.Iterations,
                        GreeedyKnapsack = simulatedAnnealingConfig.GreedyKnapsackMutator,
                        MutatorTypeSA = simulatedAnnealingConfig.Mutator.ToString(),
                        NeighborSizeSA = simulatedAnnealingConfig.NeighborhoodSize,
                        StartingTemperature = simulatedAnnealingConfig.StartingTemperature,
                        TargetTemperature = simulatedAnnealingConfig.TargetTemperature,
                        AnnealingRate = simulatedAnnealingConfig.AnnealingRate,
                        Metaheuristic = "SimulatedAnnealing",
                        FileName = simulatedAnnealingConfig.InputFileName,
                    };
                    break;
                case WavyHybridConfig:
                    var wavyHybridConfig = (WavyHybridConfig)config;
                    if(this.logger == null || this.rowsLogged == 1000000)
                    {
                        this.RecreateLogger(wavyHybridConfig.OutputFileName);
                    }
                    record = new FullSearchRecord()
                    {
                        ConfigIndex = wavyHybridConfig.ConfigIndex,
                        BestScore = results.Max(x => x.Evaluate()),
                        WorstScore = results.Min(x => x.Evaluate()),
                        AverageScore = results.Average(x => x.Evaluate()),
                        StandardError = this.CalculateStandardError(results),
                        Iterations = wavyHybridConfig.Iterations,
                        MutatorTypeSA = wavyHybridConfig.MutatorSA.ToString(),
                        MutatorTypeTS = wavyHybridConfig.MutatorTS.ToString(),
                        NeighborSizeSA = wavyHybridConfig.NeighbourhoodSizeSA,
                        NeighborSizeTS = wavyHybridConfig.NeighbourhoodSizeTS,
                        AnnealingRate = wavyHybridConfig.AnnealingRate,
                        FileName = wavyHybridConfig.InputFileName,
                        GreeedyKnapsack = wavyHybridConfig.UseGreedyKnapsack,
                        Metaheuristic = "WavyHybrid",
                        InitializatorType = wavyHybridConfig.SpecimenInitializator.Type.ToString(),
                        StartingTemperature = wavyHybridConfig.StartingTemperature,
                        StartingTemperatureChangeWavyHybrid = wavyHybridConfig.StartingTemperatureChange,
                        TabuSize = wavyHybridConfig.TabuSize,
                        TargetTemperature = wavyHybridConfig.TargetTemperature,
                        StartingMetaheuristics = wavyHybridConfig.StartingMetaheuristic.ToString(),
                        HybridIterations = wavyHybridConfig.HybridIterations
                    };
                    break;
                default:
                    return;
            }

            this.rowsLogged++;
            this.logger.Log(record);
        }

        private void RecreateLogger(string path)
        {
            if(this.logger != null)
            {
                var csvLogger = (CSVLogger<Specimen, FullSearchRecord>)this.logger;
                csvLogger.Wait();
                csvLogger.Dispose();
            }
            this.rowsLogged = 0;
            this.logger = new CSVLogger<Specimen, FullSearchRecord>(path.Insert(path.LastIndexOf('.'), $"_{this.fileIndex++}"));
            ((CSVLogger<Specimen, FullSearchRecord>)this.logger).RunLogger();
        }

        private IEnumerator<IConfig> GetConfigsEnumerator()
        {
            foreach (var filePath in this.FullSearchConfig.FilePaths)
            {
                IEnumerator<IConfig> enumerator;
                enumerator = this.GenerateTabuConfigs(filePath, this.FullSearchConfig);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                enumerator = this.GenerateEvolutionaryAlgorithmConfigs(filePath, this.FullSearchConfig);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                enumerator = this.GenerateSimulatedAnnealingConfigs(filePath, this.FullSearchConfig);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                enumerator = this.GenerateWavyConfigs(filePath, this.FullSearchConfig);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                enumerator = this.GenerateAgingHybridConfigs(filePath, this.FullSearchConfig);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }

        #region Generate
        private IEnumerator<IConfig> GenerateTabuConfigs(string filePath, FullSearchConfig fullSearchConfig)
        {
            var tabuConfig = fullSearchConfig.TabuConfig;
            if(tabuConfig == null)
            {
                yield break;
            }
            foreach (var useGreedyKnapsack in tabuConfig.UseGreedyKnapsack)
            {
                foreach(var mutator in tabuConfig.Mutators)
                {
                    foreach(var specimenInitializator in tabuConfig.SpecimenInitializators)
                    {
                        for(int neigbourhoodSize = tabuConfig.MinNeighborhoodSize; neigbourhoodSize < tabuConfig.MaxNeighborhoodSize; neigbourhoodSize += tabuConfig.NeighborhoodSizeChange)
                        {
                            for (int tabuSize = tabuConfig.MinTabuSize; tabuSize < tabuConfig.MaxTabuSize; tabuSize += tabuConfig.TabuSizeChange)
                            {
                                for (int iterations = tabuConfig.MinIterations; iterations < tabuConfig.MaxIterations; iterations += tabuConfig.IterationsChange)
                                {
                                    var config = new TabuConfig()
                                    {
                                        GreedyKnapsackMutator = useGreedyKnapsack,
                                        Mutator = mutator,
                                        Iterations = iterations,
                                        InputFileName = filePath,
                                        SpecimenInitializator = specimenInitializator,
                                        NeighborhoodSize = neigbourhoodSize,
                                        RunCount = fullSearchConfig.RunCount,
                                        TabuSize = tabuSize,
                                        OutputFileName = fullSearchConfig.OutputPath
                                    };
                                    yield return config;
                                }
                            }
                        }
                    }
                }
            }
        }
        private IEnumerator<IConfig> GenerateEvolutionaryAlgorithmConfigs(string filePath, FullSearchConfig fullSearchConfig)
        {
            FullSearchEvolutionaryAlgorithm? algorithm = fullSearchConfig.EvolutionaryAlgorithm;
            if (algorithm == null)
            {
                yield break;
            }
            foreach (var mutator in algorithm.Mutator.Types)
            {
                foreach (var crossover in algorithm.Crossover.Types)
                {
                    foreach (var selector in algorithm.Selector.Types)
                    {
                        foreach (var specimenInitializator in algorithm.SpecimenInitializators)
                        {
                            for (double mutatorProb = algorithm.Mutator.MinMutateRatio; mutatorProb < algorithm.Mutator.MaxMutateRatio; mutatorProb += algorithm.Mutator.MutateRatioChange)
                            {
                                for (double crossoverProb = algorithm.Crossover.MinProbability; crossoverProb < algorithm.Crossover.MaxProbability; crossoverProb += algorithm.Crossover.ProbabilityChange)
                                {
                                    for (int specimenCount = algorithm.Selector.MinSpecimenCount; specimenCount < algorithm.Selector.MaxSpecimenCount; specimenCount += algorithm.Selector.SpecimenCountChange)
                                    {
                                        for (int populationSize = algorithm.MinPopulationSize; populationSize < algorithm.MaxPopulationSize; populationSize += algorithm.PopulationSizeChange)
                                        {
                                            for (int epochs = algorithm.MinEpochs; epochs < algorithm.MaxEpochs; epochs += algorithm.EpochsChange)
                                            {
                                                var config = new LearningConfig()
                                                {
                                                    Mutator = new MutatorConfig()
                                                    {
                                                        Type = mutator,
                                                        MutateRatio = mutatorProb
                                                    },
                                                    Selector = new SelectorConfig()
                                                    {
                                                        Type = selector,
                                                        SpecimenCount = specimenCount,
                                                    },
                                                    Crossover = new CrossoverConfig()
                                                    {
                                                        Type = crossover,
                                                        Probability = crossoverProb,
                                                    },
                                                    InputFileName = filePath,
                                                    Epochs = epochs,
                                                    PopulationSize = populationSize,
                                                    RunCount = fullSearchConfig.RunCount,
                                                    SpecimenInitializator = specimenInitializator,
                                                    OutputFileName = fullSearchConfig.OutputPath
                                                };
                                                yield return config;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }                  
                }
            }
        }

        private IEnumerator<IConfig> GenerateAgingHybridConfigs(string filePath, FullSearchConfig fullSearchConfig)
        {
            FullSearchAgingHybrid? agingHybridConfig = fullSearchConfig.AgingHybrid;
            if (agingHybridConfig == null)
            {
                yield break;
            }
            foreach (var mutator in agingHybridConfig.Mutator.Types)
            {
                foreach (var crossover in agingHybridConfig.Crossover.Types)
                {
                    foreach (var selector in agingHybridConfig.Selector.Types)
                    {
                        foreach (var specimenInitializator in agingHybridConfig.SpecimenInitializators)
                        {
                            foreach (double mutatorProb in agingHybridConfig.Mutator.MutateRatios)
                            {
                                foreach (double crossoverProb in agingHybridConfig.Crossover.Probabilities)
                                {
                                    foreach (int specimenCount in agingHybridConfig.Selector.SpecimenCounts)
                                    {
                                        foreach (int populationSize in agingHybridConfig.PopulationSizes)
                                        {
                                            foreach (int epochs in agingHybridConfig.Epochs)
                                            {
                                                foreach(int age in agingHybridConfig.Ages)
                                                {
                                                    foreach(int ageVariety in agingHybridConfig.AgeVarieties)
                                                    {
                                                        var config = new AgingHybridConfig()
                                                        {
                                                            Mutator = new MutatorConfig()
                                                            {
                                                                Type = mutator,
                                                                MutateRatio = mutatorProb
                                                            },
                                                            Selector = new SelectorConfig()
                                                            {
                                                                Type = selector,
                                                                SpecimenCount = specimenCount,
                                                            },
                                                            Crossover = new CrossoverConfig()
                                                            {
                                                                Type = crossover,
                                                                Probability = crossoverProb,
                                                            },
                                                            InputFileName = filePath,
                                                            Epochs = epochs,
                                                            PopulationSize = populationSize,
                                                            RunCount = fullSearchConfig.RunCount,
                                                            SpecimenInitializator = specimenInitializator,
                                                            OutputFileName = fullSearchConfig.OutputPath,
                                                            Age = age,
                                                            AgeVariety = ageVariety,
                                                            UseLogging = agingHybridConfig.UseLogging,
                                                            LoggingTemplatePath = agingHybridConfig.LoggingTemplatePath
                                                        };
                                                        yield return config;
                                                    }
                                                }                                                                                           
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator<IConfig> GenerateWavyConfigs(string filePath, FullSearchConfig fullSearchConfig)
        {
            var wavyHybrid = fullSearchConfig.WavyHybrid;
            if (wavyHybrid == null)
            {
                yield break;
            }
            foreach (var mutatorSA in wavyHybrid.MutatorsSA)
            {
                foreach (var mutatorTS in wavyHybrid.MutatorsTS)
                {
                    foreach (var specimenInitializator in wavyHybrid.SpecimenInitializators)
                    {
                        foreach (var neigbourhoodSizeSA in wavyHybrid.NeighbourhoodSizeSA)
                        {
                            foreach (var neigbourhoodSizeTS in wavyHybrid.NeighbourhoodSizeTS)
                            {
                                foreach (var greedyKnapsack in wavyHybrid.UseGreedyKnapsack)
                                {
                                    foreach (var annealingRate in wavyHybrid.AnnealingRate)
                                    {
                                        foreach (var hybridIteration in wavyHybrid.HybridIterations)
                                        {
                                            foreach (var startingTemperature in wavyHybrid.StartingTemperature)
                                            {
                                                foreach (var startingTemperatureChange in wavyHybrid.StartingTemperatureChange)
                                                {
                                                    foreach (var targetTemperature in wavyHybrid.TargetTemperature)
                                                    {
                                                        foreach (var iterations in wavyHybrid.Iterations)
                                                        {
                                                            foreach (var tabuSize in wavyHybrid.TabuSize)
                                                            {
                                                                foreach(var startingMetaheuristic in wavyHybrid.StartingMetaheuristicTypes)
                                                                {
                                                                    yield return new WavyHybridConfig()
                                                                    {
                                                                        ConfigIndex = fileIndex++,
                                                                        AnnealingRate = annealingRate,
                                                                        MutatorSA = mutatorSA,
                                                                        MutatorTS = mutatorTS,
                                                                        HybridIterations = hybridIteration,
                                                                        InputFileName = filePath,
                                                                        OutputFileName = fullSearchConfig.OutputPath,
                                                                        Iterations = iterations,
                                                                        NeighbourhoodSizeSA = neigbourhoodSizeSA,
                                                                        NeighbourhoodSizeTS = neigbourhoodSizeTS,
                                                                        RunCount = fullSearchConfig.RunCount,
                                                                        SpecimenInitializator = specimenInitializator,
                                                                        StartingTemperature = startingTemperature,
                                                                        TabuSize = tabuSize,
                                                                        TargetTemperature = targetTemperature,
                                                                        StartingTemperatureChange = startingTemperatureChange,
                                                                        UseGreedyKnapsack = greedyKnapsack,
                                                                        AdditionalLoggingPathTemplate = wavyHybrid.AdditionalLoggingPathTemplate,
                                                                        UseAdditionalLogging = wavyHybrid.UseAdditionalLogging,
                                                                        UseLogging = wavyHybrid.UseLogging,
                                                                        LoggingPathTemplate = wavyHybrid.LoggingPathTemplate,
                                                                        StartingMetaheuristic = startingMetaheuristic,
                                                                    };
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator<IConfig> GenerateSimulatedAnnealingConfigs(string filePath, FullSearchConfig fullSearchConfig)
        {
            FullSearchSimulatedAnnealing? simulatedAnnealing = fullSearchConfig.SimulatedAnnealing;
            if (simulatedAnnealing == null)
            {
                yield break;
            }
            foreach (var useGreedyKnapsack in simulatedAnnealing.UseGreedyKnapsack)
            {
                foreach (var mutator in simulatedAnnealing.Mutators)
                {
                    foreach (var specimenInitializator in simulatedAnnealing.SpecimenInitializators)
                    {
                        for (int neigbourhoodSize = simulatedAnnealing.MinNeighborhoodSize; neigbourhoodSize < simulatedAnnealing.MaxNeighborhoodSize; neigbourhoodSize += simulatedAnnealing.NeighborhoodSizeChange)
                        {
                            for (double startingTemperature = simulatedAnnealing.MinStartingTemperature; startingTemperature < simulatedAnnealing.MaxStartingTemperature; startingTemperature += simulatedAnnealing.StartingTemperatureChange)
                            {
                                for (double targetTemperature = simulatedAnnealing.MinTargetTemperature; targetTemperature < simulatedAnnealing.MaxTargetTemperature; targetTemperature += simulatedAnnealing.TargetTemperatureChange)
                                {
                                    for (double annealingRate = simulatedAnnealing.MinAnnealingRate; annealingRate < simulatedAnnealing.MaxAnnealingRate; annealingRate += simulatedAnnealing.AnnealingRateChange)
                                    {
                                        for (int iterations = simulatedAnnealing.MinIterations; iterations < simulatedAnnealing.MaxIterations; iterations += simulatedAnnealing.IterationsChange)
                                        {
                                            var config = new SimulatedAnnealingConfig()
                                            {
                                                GreedyKnapsackMutator = useGreedyKnapsack,
                                                Mutator = mutator,
                                                Iterations = iterations,
                                                InputFileName = filePath,
                                                SpecimenInitializator = specimenInitializator,
                                                NeighborhoodSize = neigbourhoodSize,
                                                RunCount = fullSearchConfig.RunCount,
                                                StartingTemperature = startingTemperature,
                                                TargetTemperature = targetTemperature,
                                                AnnealingRate = annealingRate,
                                                OutputFileName = fullSearchConfig.OutputPath
                                            };
                                            yield return config;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        private List<Specimen> RunTabuSearch(TabuConfig tabuConfig)
        {
            var results = new List<Specimen>();
            for(int i = 0; i < tabuConfig.RunCount; i++)
            {
                Data? data;
                lock (this.LoadedData)
                {
                    if (this.LoadedData.ContainsKey(tabuConfig.InputFileName))
                    {
                        data = this.LoadedData[tabuConfig.InputFileName];
                    }
                    else
                    {
                        var dataLoader = new DataLoader();
                        data = dataLoader.Load(tabuConfig.InputFileName);
                        this.LoadedData.Add(tabuConfig.InputFileName, data);
                    }
                }       
                ISpecimenInitializator<Specimen> initializator;
                if (tabuConfig.SpecimenInitializator.Type == SpecimenInitializatorType.Greedy)
                {
                    initializator = new GreedySpecimenInitializator(data, new KnapsackMutator(data, true));
                }
                else
                {
                    initializator = new RandomSpecimenInitializator(data, tabuConfig.SpecimenInitializator.ItemAddPropability);
                }
                var factory = new SpecimenFactory(data, initializator);
                IMutator<Specimen> mutator;
                if (tabuConfig.Mutator == MutatorType.Swap)
                {
                    mutator = new TabuSwapMutator(data);
                }
                else
                {
                    mutator = new InverseMutator(data, 1);
                }
                var knapsackMutator = new KnapsackMutator(data, tabuConfig.GreedyKnapsackMutator);
                var neighbourhood = new Neighbourhood(mutator, knapsackMutator);

                var tabuSearch = new TabuSearchManager(factory, neighbourhood, null, tabuConfig.Iterations, tabuConfig.NeighborhoodSize, tabuConfig.TabuSize);
                results.Add(tabuSearch.RunTabuSearch());
            }
            return results;
        }

        private List<Specimen> RunSimulatedAnnealing(SimulatedAnnealingConfig simulatedAnnealingConfig)
        {
            var results = new List<Specimen>();
            for (int i = 0; i < simulatedAnnealingConfig.RunCount; i++)
            {
                Data? data;
                lock (this.LoadedData)
                {
                    if (this.LoadedData.ContainsKey(simulatedAnnealingConfig.InputFileName))
                    {
                        data = this.LoadedData[simulatedAnnealingConfig.InputFileName];
                    }
                    else
                    {
                        var dataLoader = new DataLoader();
                        data = dataLoader.Load(simulatedAnnealingConfig.InputFileName);
                        this.LoadedData.Add(simulatedAnnealingConfig.InputFileName, data);
                    }
                }
                ISpecimenInitializator<Specimen> initializator;
                if (simulatedAnnealingConfig.SpecimenInitializator.Type == SpecimenInitializatorType.Greedy)
                {
                    initializator = new GreedySpecimenInitializator(data, new KnapsackMutator(data, true));
                }
                else
                {
                    initializator = new RandomSpecimenInitializator(data, simulatedAnnealingConfig.SpecimenInitializator.ItemAddPropability);
                }
                var factory = new SpecimenFactory(data, initializator);
                IMutator<Specimen> mutator;
                if (simulatedAnnealingConfig.Mutator == MutatorType.Swap)
                {
                    mutator = new TabuSwapMutator(data);
                }
                else
                {
                    mutator = new InverseMutator(data, 1);
                }
                var knapsackMutator = new KnapsackMutator(data, simulatedAnnealingConfig.GreedyKnapsackMutator);
                var neighbourhood = new Neighbourhood(mutator, knapsackMutator);

                var simulatedAnnealing = new SimulatedAnnealingManager(neighbourhood
                    , factory
                    , null
                    , simulatedAnnealingConfig.AnnealingRate
                    , simulatedAnnealingConfig.Iterations
                    , simulatedAnnealingConfig.NeighborhoodSize
                    , simulatedAnnealingConfig.StartingTemperature
                    , simulatedAnnealingConfig.TargetTemperature
                    );
                results.Add(simulatedAnnealing.RunSimulatedAnnealing());
            }
            return results;
        }

        private List<Specimen> RunEvolutionaryAlgorithm(LearningConfig learningConfig)
        {
            var results = new List<Specimen>();
            for (int i = 0; i < learningConfig.RunCount; i++)
            {
                Data? data;
                lock (this.LoadedData)
                {
                    if (this.LoadedData.ContainsKey(learningConfig.InputFileName))
                    {
                        data = this.LoadedData[learningConfig.InputFileName];
                    }
                    else
                    {
                        var dataLoader = new DataLoader();
                        data = dataLoader.Load(learningConfig.InputFileName);
                        this.LoadedData.Add(learningConfig.InputFileName, data);
                    }
                }
                if (data == null)
                {
                    Environment.Exit(-1);
                }

                IMutator<Specimen> mutator;
                ISpecimenInitializator<Specimen> specimenInitializator;
                ISelector<Specimen> selector;
                ICrossover<Specimen> crossover;

                if (learningConfig.Mutator.Type == MutatorType.Swap)
                {
                    mutator = new SwapMutator(data, learningConfig.Mutator.MutateRatio);
                }
                else
                {
                    mutator = new InverseMutator(data, learningConfig.Mutator.MutateRatio);
                }

                if (learningConfig.SpecimenInitializator.Type == SpecimenInitializatorType.Random)
                {
                    specimenInitializator = new RandomSpecimenInitializator(data, learningConfig.SpecimenInitializator.ItemAddPropability);
                }
                else
                {
                    specimenInitializator = new GreedySpecimenInitializator(data, new KnapsackMutator(data, true));
                }

                if (learningConfig.Selector.Type == SelectionType.Roulette)
                {
                    selector = new RouletteSelection<Specimen>(learningConfig.Selector.IsMinimalizing);
                }
                else
                {
                    selector = new TournamentSelection<Specimen>(learningConfig.Selector.SpecimenCount, learningConfig.Selector.IsMinimalizing);
                }

                if (learningConfig.Crossover.Type == CrossoverType.Order)
                {
                    crossover = new OrderCrossover(learningConfig.Crossover.Probability);
                }
                else
                {
                    crossover = new PartiallyMatchedCrossover(learningConfig.Crossover.Probability);
                }

                var additionalOperations = new AdditionalOperationsHandler(new KnapsackMutator(data, true));
                var specimenFactory = new SpecimenFactory(data, specimenInitializator);

                var learningManager = new EAManager(data
                    , mutator
                    , crossover
                    , selector
                    , specimenFactory
                    , (uint)learningConfig.PopulationSize
                    , learningConfig.Epochs
                    , null
                    , additionalOperations
                    );
                learningManager.Init();
                for (int j = 0; j < learningConfig.Epochs; j++)
                {
                    learningManager.NextEpoch();
                }
                results.Add(learningManager.Best);
            }
            return results;
        }

        private List<Specimen> RunAgingHybrid(AgingHybridConfig config)
        {
            var results = new List<Specimen>();
            var factory = new AgingHybridManagerFactory();
            var mapper = MapperProfile.Mapper;
            Parallel.For(0, config.RunCount, (i, state) =>
            {
                CSVLogger<Specimen, WavyHybridRecord>? logger = null;
                string path;
                if (config.UseLogging)
                {
                    lock (this.additionalLoggingLock)
                    {
                        if (!this.additionalLoggingIndexes.ContainsKey(config.LoggingTemplatePath))
                        {
                            this.additionalLoggingIndexes.Add(config.LoggingTemplatePath, 0);
                        }
                        path = string.Format(config.LoggingTemplatePath, this.additionalLoggingIndexes[config.LoggingTemplatePath]++);
                    }
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
                var specimen = manager.RunManager();
                results.Add(mapper.Map<SpecimenWithAge, Specimen>(specimen));

                logger?.Wait();
                logger?.Dispose();
            });
            return results;
        }

        private List<Specimen> RunWavyHybrid(WavyHybridConfig config)
        {
            var results = new List<Specimen>();
            var factory = new WavyHybridManagerFactory();
            Parallel.For(0, config.RunCount, (i, state) =>
            {
                CSVLogger<Specimen, WavyHybridRecord>? logger = null;
                CSVLogger<Specimen, WavyHybridRecord>? innerLogger;
                if (config.UseLogging)
                {
                    var path = string.Format(config.LoggingPathTemplate, config.ConfigIndex, i);
                    logger = new CSVLogger<Specimen, WavyHybridRecord>(path);
                }
                //local function
                void Manager_RecordCreated(object sender, WavyHybridRecord e)
                {
                    logger?.Log(e);
                }
                //
                logger?.RunLogger();
                var innerPath = string.Format(config.AdditionalLoggingPathTemplate, config.ConfigIndex, i);
                var manager = factory.Create(config, out innerLogger, innerPath);

                manager.RecordCreated += Manager_RecordCreated;
                var specimen = manager.RunManager();
                results.Add(specimen);

                innerLogger?.Wait();
                innerLogger?.Dispose();
                logger?.Wait();
                logger?.Dispose();
                manager.RecordCreated -= Manager_RecordCreated;
            });
            return results;
        }

        public void Wait()
        {
            this.managerTask.Wait();
        }

        public void Dispose()
        {
            this.cancellationToken.Cancel();
            this.managerTask = null;
        }
    }
}
