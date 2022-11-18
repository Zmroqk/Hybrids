using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public interface IConfig
    {
        public string[] Include { get; set; } 
    }
}
