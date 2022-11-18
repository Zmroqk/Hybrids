using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Config
{
    public class LearningConfigLoader<TConfig> where TConfig : IConfig
    {
        public List<TConfig> Load(string path)
        {
            if (File.Exists(path))
            {
                var configs = JsonConvert.DeserializeObject<List<TConfig>>(File.ReadAllText(path));
                var configsWithImport = configs.Where(c => c.Include != null).ToList();
                foreach(var config in configsWithImport)
                {
                    configs.Remove(config);
                    foreach(var innerPath in config.Include)
                    {
                        var additionalConfigs = this.Load(innerPath);
                        configs.AddRange(additionalConfigs);
                    }
                }
                return configs;
            }
            else
            {
                Environment.Exit(-3);
                return null;
            }
        }
    }
}
