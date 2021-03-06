﻿using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes
{
    public class ProgrammeCategory : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
