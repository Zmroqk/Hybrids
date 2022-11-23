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
            var specimen = new SpecimenWithAge(this.Config, this.SpecimenInitialization);
            specimen.Nodes = new List<Node>(this.Nodes);
            specimen.Items = new HashSet<Item>(this.Items);
            specimen.IsMutated = this.IsMutated;
            specimen.IsCrossed = this.IsCrossed;;
            specimen.Age = Age;
            return specimen;
        }

        public override bool Equals(SpecimenWithAge? other)
        {
            if (other == null)
            {
                return false;
            }
            if(this.Age != other.Age)
            {
                return false;
            }
            if (this.Nodes.Count != other.Nodes.Count || this.Items.Count != other.Items.Count)
            {
                return false;
            }
            var nodeCount = this.Nodes.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                if (this.Nodes[i] != other.Nodes[i])
                {
                    return false;
                }
            }
            foreach (var item in this.Items)
            {
                if (!other.Items.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
