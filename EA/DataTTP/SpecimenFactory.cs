﻿using EA.EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.DataTTP
{
    public class SpecimenFactory : ISpecimenFactory<Specimen>
    {
        private readonly Data config;
        private readonly ISpecimenInitializator<Specimen> specimenInitialization;

        public SpecimenFactory(Data config, ISpecimenInitializator<Specimen> specimenInitialization)
        {
            this.config = config;
            this.specimenInitialization = specimenInitialization;
        }

        public Specimen CreateSpecimen()
        {
            var specimen = new Specimen(this.config, this.specimenInitialization);
            specimen.Init();
            return specimen;
        }
    }
}
