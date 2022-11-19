using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public class SpecimenWithAgeFactory : ISpecimenFactory<SpecimenWithAge>
    {
        private readonly Data config;
        private readonly ISpecimenInitializator<SpecimenWithAge> specimenInitialization;

        public SpecimenWithAgeFactory(Data config, ISpecimenInitializator<SpecimenWithAge> specimenInitialization)
        {
            this.config = config;
            this.specimenInitialization = specimenInitialization;
        }

        public SpecimenWithAge CreateSpecimen()
        {
            var specimen = new SpecimenWithAge(this.config, this.specimenInitialization);
            specimen.Init();
            return specimen;
        }
    }
}
