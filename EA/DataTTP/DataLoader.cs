using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public class DataLoader : IDataLoader<Data>
    {
        public static DataLoader Instance { get; } = new DataLoader();

        private Dictionary<string, Data> Cache;
        public DataLoader()
        {
            this.Cache = new Dictionary<string, Data>();
        }

        public Data GetData(string path)
        {
            lock (this.Cache)
            {
                if (this.Cache.ContainsKey(path))
                {
                    return this.Cache[path];
                }
                else
                {
                    var data = this.Load(path);
                    if (data == null)
                    {
                        Environment.Exit(-1);
                    }
                    this.Cache.Add(path, data);
                    return data;
                }
            }
        }

        public Data? Load(string path)
        {
            string[] lines = File.ReadAllLines(path);
            try
            {
                var data = new Data();
                data.Name = lines[0].Split('\t')[1];
                data.Type = lines[1].Split(':')[1].Trim();
                data.CityCount = int.Parse(lines[2].Split('\t')[1]);
                data.NumberOfItems = int.Parse(lines[3].Split('\t')[1]);
                data.KnapsackCapacity = int.Parse(lines[4].Split('\t')[1]);
                var decimalFormat = new NumberFormatInfo() { CurrencyDecimalSeparator = "." };
                data.MinSpeed = double.Parse(lines[5].Split('\t')[1], decimalFormat);
                data.MaxSpeed = double.Parse(lines[6].Split('\t')[1], decimalFormat);
                data.RentingRatio = double.Parse(lines[7].Split('\t')[1], decimalFormat);
                data.EdgeWeightType = lines[8].Split('\t')[1];
                for(int i = 10; i < 10 + data.CityCount; i++)
                {
                    var lineData = lines[i].Split('\t');
                    data.Nodes.Add(new Node(int.Parse(lineData[0])
                        , double.Parse(lineData[1], decimalFormat)
                        , double.Parse(lineData[2], decimalFormat)
                        )
                    );
                }
                for (int i = 11 + data.CityCount; i < 11 + data.NumberOfItems + data.CityCount; i++)
                {
                    var lineData = lines[i].Split('\t');
                    var nodeIndex = int.Parse(lineData[3]);
                    var node = data.Nodes.First(n => n.Index == nodeIndex);
                    var item = new Item(int.Parse(lineData[0])
                        , int.Parse(lineData[1])
                        , int.Parse(lineData[2])
                        , nodeIndex
                        , node
                        );
                    data.Items.Add(item);
                    node.AvailableItems.Add(item);
                }
                data.GenerateDistances();
                return data;
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Incorrect format");
                return null;
            }
        }
    }
}
