using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace xggameplan.AuditEvents
{
    public class AuditEventModel
    {
        [JsonProperty("ID")]
        public string ID { get; set; }

        [JsonProperty("TimeCreated")]
        public DateTime TimeCreated { get; set; }

        [JsonProperty("TenantID")]
        public int TenantID { get; set; }

        [JsonProperty("UserID")]
        public int UserID { get; set; }

        [JsonProperty("Source")]
        public string Source { get; set; }

        [JsonProperty("EventTypeID")]
        public int EventTypeID { get; set; }

        [JsonProperty("Values")]
        public List<AuditEventValueModel> Values = new List<AuditEventValueModel>();
    }
}
