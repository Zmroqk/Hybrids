using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Mutators
{
    public class TabuSwapMutator : SwapMutator
    {
        Random random;
        public TabuSwapMutator(Data config) : base(config, 1)
        {
            this.random = new Random();
        }

        public override Specimen Mutate(Specimen specimen)
        {         
            var index = random.Next(specimen.Nodes.Count);
            var index2 = random.Next(specimen.Nodes.Count);
            var swappedNode = specimen.Nodes[index];
            specimen.Nodes[index] = specimen.Nodes[index2];
            specimen.Nodes[index2] = swappedNode;
            specimen.IsMutated = true;
            return specimen;
        }
    }
}
