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
    public class GreedySpecimenInitializator : ISpecimenInitializator<Specimen>
    {
        public Data Config { get; set; }
        public IMutator<Specimen> KnapsackMutator { get; set; }
        Random random;
        List<Node> cities;

        public GreedySpecimenInitializator(Data config, IMutator<Specimen> knapsackMutator)
        {
            this.Config = config;
            this.KnapsackMutator = knapsackMutator;
            this.random = new Random();
            this.cities = this.Config.Nodes.ToList();
        }

        public void Initialize(Specimen specimen)
        {
            var citiesHash = this.Config.Nodes.ToHashSet();
            var distanceMatrix = this.Config.GetNodeMatrix();
            var currentCity = cities[random.Next(0, cities.Count)];
            citiesHash.Remove(currentCity);
            specimen.Nodes.Add(currentCity);
            while (citiesHash.Count > 0)
            {
                var infos = distanceMatrix[currentCity.Index-1];
                var maxDistance = 0d;
                var selectedInfo = infos[0];
                foreach(var info in infos)
                {
                    if(info.From != info.To && citiesHash.Contains(info.To) && maxDistance < info.Distance)
                    {
                        selectedInfo = info;
                        maxDistance = info.Distance;
                    }
                }
                currentCity = selectedInfo.To;
                citiesHash.Remove(currentCity);
                specimen.Nodes.Add(currentCity);
            }
            this.KnapsackMutator.Mutate(specimen);
        }
    }
}
