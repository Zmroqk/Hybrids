﻿using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Core
{
    public interface IMutator<T> where T : ISpecimen<T>
    {
        double Probability { get; set; }
        IList<T> MutateAll(IList<T> currentPopulation);
        T Mutate(T specimen);
    }
}
