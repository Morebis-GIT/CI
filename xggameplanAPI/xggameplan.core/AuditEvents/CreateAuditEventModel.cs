using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace xggameplan.AuditEvents
{
    public class CreateAuditEventModel
    {
        [JsonProperty("EventTypeID")]
        public int EventTypeID { get; set; }

        [JsonProperty("TimeCreated")]
        public DateTime? TimeCreated { get; set; }

        [JsonProperty("Values")]
        public List<AuditEventValueModel> Values = new List<AuditEventValueModel>();
    }

    internal class CreateAuditEventModelProfile : AutoMapper.Profile
    {
        public CreateAuditEventModelProfile()
        {
            CreateMap<AuditEvent, CreateAuditEventModel>();
        }
    }
}
