using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Inititializators
{
    public class RandomSpecimenWithAgeInitializator : RandomSpecimenInitializatorBase<SpecimenWithAge>
    {
        public RandomSpecimenWithAgeInitializator(Data config, double itemAddProbability, int ageForNewSpecimen, int ageVariety) : base(config, itemAddProbability)
        {
            this.AgeForNewSpecimen = ageForNewSpecimen;
            this.AgeVariety = ageVariety;
        }

        public int AgeForNewSpecimen { get; set; }
        public int AgeVariety { get; set; }



        public override void Initialize(SpecimenWithAge specimen)
        {
            base.Initialize(specimen);
            specimen.Age = this.random.Next(this.AgeForNewSpecimen - this.AgeVariety, this.AgeForNewSpecimen + this.AgeVariety);
        }
    }
}
