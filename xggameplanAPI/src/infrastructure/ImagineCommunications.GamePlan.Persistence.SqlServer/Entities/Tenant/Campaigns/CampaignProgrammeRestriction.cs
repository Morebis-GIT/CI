using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignProgrammeRestriction : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid CampaignId { get; set; }
        public CategoryOrProgramme IsCategoryOrProgramme { get; set; }
        public IncludeOrExclude IsIncludeOrExclude { get; set; }

        public ICollection<CampaignProgrammeRestrictionCategoryOrProgramme> CategoryOrProgramme { get; set; } =
            new HashSet<CampaignProgrammeRestrictionCategoryOrProgramme>();
        public ICollection<CampaignProgrammeRestrictionSalesArea> SalesAreas { get; set; } =
            new HashSet<CampaignProgrammeRestrictionSalesArea>();
    }
}
