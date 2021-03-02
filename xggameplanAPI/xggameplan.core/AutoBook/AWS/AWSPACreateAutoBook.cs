using Newtonsoft.Json;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// Details for creating new AutoBook object in AutoBooks API (AWS)
    /// </summary>
    public class AWSPACreateAutoBook
    {
        [JsonProperty("instanceType")]
        public string InstanceType { get; set; }

        [JsonProperty("storageSizeGb")]
        public int StorageSizeGb { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
