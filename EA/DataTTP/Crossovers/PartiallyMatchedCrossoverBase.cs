using EA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP.Crossovers
{
    public class PartiallyMatchedCrossoverBase<TSpecimen> : ICrossover<TSpecimen> where TSpecimen : TTPSpecimenBase<TSpecimen>
    {
        public double Probability { get; set; }
        Random random;
        public PartiallyMatchedCrossoverBase(double probability)
        {
            this.Probability = probability;
            this.random = new Random();
        }

        public IList<TSpecimen> Crossover(IList<TSpecimen> specimens)
        {
            var prop = 1 - this.Probability;
            var newSpecimens = new List<TSpecimen>();
            for (int i = 1; i < specimens.Count; i += 2)
            {
                if (prop <= random.NextDouble())
                {
                    var (specimen1, specimen2) = this.CrossSpecimens(specimens[i - 1], specimens[i]);
                    specimen1.IsCrossed = true;
                    specimen2.IsCrossed = true;
                    newSpecimens.Add(specimen1);
                    newSpecimens.Add(specimen2);
                }
                else
                {
                    newSpecimens.Add(specimens[i - 1].Clone());
                    newSpecimens.Add(specimens[i].Clone());
                }
            }
            if(specimens.Count % 2 != 0)
            {
                specimens.RemoveAt(specimens.Count - 1);
            }
            return newSpecimens;
        }

        private (TSpecimen, TSpecimen) CrossSpecimens(TSpecimen specimen, TSpecimen otherSpecimen)
        {
            var newSpecimen = specimen.Clone();
            var newOtherSpecimen = otherSpecimen.Clone();
            var startIndex = random.Next(specimen.Nodes.Count);
            var length = random.Next(specimen.Nodes.Count - startIndex + 1);
            var specimenRange = newSpecimen.Nodes.GetRange(startIndex, length);
            var otherSpecimenRange = newOtherSpecimen.Nodes.GetRange(startIndex, length);

            var mapping1 = this.CreateMapping(specimenRange, otherSpecimenRange);
            var mapping2 = this.CreateMapping(otherSpecimenRange, specimenRange);

            newSpecimen.Nodes.RemoveRange(startIndex, length);
            newSpecimen.Nodes.InsertRange(startIndex, otherSpecimenRange);
            newOtherSpecimen.Nodes.RemoveRange(startIndex, length);
            newOtherSpecimen.Nodes.InsertRange(startIndex, specimenRange);

            for (int i = 0; i < newSpecimen.Nodes.Count; i++)
            {
                if (i < startIndex || startIndex + length < i)
                {
                    var city = newSpecimen.Nodes[i];
                    while (mapping2.ContainsKey(city))
                    {
                        var newCity = mapping2[city];
                        if(newCity == newSpecimen.Nodes[i])
                        {
                            break;
                        }
                        city = newCity;
                    }
                    if (newSpecimen.Nodes[i] != city)
                    {
                        newSpecimen.Nodes[i] = city;
                    }
                }
            }
            for (int i = 0; i < newOtherSpecimen.Nodes.Count; i++)
            {
                if (i < startIndex || startIndex + length < i)
                {
                    var city = newOtherSpecimen.Nodes[i];
                    while (mapping1.ContainsKey(city))
                    {
                        var newCity = mapping1[city];
                        if (newCity == newOtherSpecimen.Nodes[i])
                        {
                            break;
                        }
                        city = newCity;
                    }
                    if (newOtherSpecimen.Nodes[i] != city)
                    {
                        newOtherSpecimen.Nodes[i] = city;
                    }
                }
            }

            newSpecimen.Fix();
            newOtherSpecimen.Fix();
            return (newSpecimen, newOtherSpecimen);
        }

        private Dictionary<Node, Node> CreateMapping(List<Node> map1, List<Node> map2)
        {
            var mapping = new Dictionary<Node, Node>();
            for(int i = 0; i < map1.Count; i++)
            {
                var node = map1[i];
                mapping.Add(node, map2[i]);
            }
            return mapping;
        }
    }
}
