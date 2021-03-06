﻿using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignPayback : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid CampaignId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }
}
