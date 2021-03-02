using Newtonsoft.Json;

namespace xggameplan.AuditEvents
{
    public class AuditEventValueModel
    {
        [JsonProperty("TypeID")]
        public int TypeID { get; set; }

        [JsonProperty("Value")]
        public object Value { get; set; }
    }
}
