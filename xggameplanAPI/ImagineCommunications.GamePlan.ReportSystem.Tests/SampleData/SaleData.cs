using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.SampleData
{
    public class SaleData
    {
        public int id { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public decimal amount { get; set; }
        public DateTime date { get; set; }

        public static List<SaleData> GetSales()
        {
            #region json region
            var assembly = Assembly.GetExecutingAssembly();
            var uri = new UriBuilder(assembly.CodeBase);
            string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));

            string value = File.ReadAllText(Path.Combine(path, "Data/sales-data.json"));

            #endregion

            return JsonConvert.DeserializeObject<List<SaleData>>(value);
        }
    }
}
