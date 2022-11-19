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
    public class GreedySpecimenInitializator<TSpecimen> : ISpecimenInitializator<TSpecimen> where TSpecimen : TTPSpecimenBase<TSpecimen>
    {
        public Data Config { get; set; }
        public IMutator<TSpecimen> KnapsackMutator { get; set; }
        protected Random random;
        protected List<Node> cities;

        public GreedySpecimenInitializator(Data config, IMutator<TSpecimen> knapsackMutator)
        {
            this.Config = config;
            this.KnapsackMutator = knapsackMutator;
            this.random = new Random();
            this.cities = this.Config.Nodes.ToList();
        }

        public virtual void Initialize(TSpecimen specimen)
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
