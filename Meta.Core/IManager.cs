using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Core
{
    public interface IManager<TSpecimen> where TSpecimen : ISpecimen<TSpecimen>
    {
        public TSpecimen RunManager();
    }
}
