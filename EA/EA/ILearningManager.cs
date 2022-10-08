﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.EA
{
    public interface ILearningManager<T> where T : ISpecimen<T>
    {
        IList<T> CurrentEpochSpecimens { get; set; }
        IMutator<T> Mutator { get; set; }
        ICrossover<T> Crossover { get; set; }
        ISelector<T> Selector { get; set; }
        ISpecimenFactory<T> SpecimenFactory { get; set; }
        ILogger<T>? Logger { get; set; }

        void Init();
        void NextEpoch();

        uint PopulationSize { get; set; }
    }
}
