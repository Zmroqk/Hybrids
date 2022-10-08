﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.EA
{
    public interface ISpecimen<T> where T : ISpecimen<T>
    {
        ISpecimenInitializator<T> SpecimenInitialization { get; }

        void Init();
        void Fix();
        double Evaluate();
    }
}
