using Meta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public class SpecimenWithAge : TTPSpecimenBase<SpecimenWithAge>
    {
        public int Age { get; set; }

        public SpecimenWithAge(Data config, ISpecimenInitializator<SpecimenWithAge> specimenInitialization) : base(config, specimenInitialization)
        {
        }

        public override SpecimenWithAge Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(SpecimenWithAge? other)
        {
            throw new NotImplementedException();
        }
    }
}
