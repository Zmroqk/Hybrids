using EA.Core;
using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.DataTTP.Mutators;

namespace Meta.DataTTP.Inititializators
{
    public class GreedySpecimenInitializatorWithAge : GreedySpecimenInitializator<SpecimenWithAge>
    {
        public int AgeForNewSpecimen { get; set; }
        public int AgeVariety { get; set; }

        public GreedySpecimenInitializatorWithAge(Data config, IMutator<SpecimenWithAge> knapsackMutator, int ageForNewSpecimen, int ageVariety) : base(config, knapsackMutator)
        {
            this.AgeForNewSpecimen = ageForNewSpecimen;
            this.AgeVariety = ageVariety;
        }

        public override void Initialize(SpecimenWithAge specimen)
        {
            base.Initialize(specimen);
            specimen.Age = this.random.Next(this.AgeForNewSpecimen - this.AgeVariety, this.AgeForNewSpecimen + this.AgeVariety);
        }
    }
}
