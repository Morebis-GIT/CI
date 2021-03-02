﻿namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings
{
    public class MinimumDocumentRestriction
    {
        public int Campaigns { get; set; }
        public int Clashes { get; set; }
        public int ClearanceCodes { get; set; }
        public int Demographics { get; set; }
        public int Products { get; set; }
    }
}