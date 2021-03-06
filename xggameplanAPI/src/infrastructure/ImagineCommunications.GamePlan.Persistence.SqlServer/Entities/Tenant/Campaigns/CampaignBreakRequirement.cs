﻿using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignBreakRequirement : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid CampaignId { get; set; }
        public Guid SalesAreaId { get; set; }
        public CampaignCentreBreakRequirementItem CentreBreakRequirement { get; set; }
        public CampaignEndBreakRequirementItem EndBreakRequirement { get; set; }
        public SalesArea SalesArea { get; set; }
    }
}
