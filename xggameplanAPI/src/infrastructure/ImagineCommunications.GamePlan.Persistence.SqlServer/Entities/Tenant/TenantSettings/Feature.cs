using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings
{
    public class Feature : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string IdValue { get; set; }

        public Guid TenantSettingsId { get; set; }

        public bool Enabled { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
