using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AgStrikeWeight : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgCampaignSalesAreaId { get; set; }

        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NbrAgStrikeWeightLengths { get; set; }
        public int SpotMaxRatings { get; set; }
        public List<AgStrikeWeightLength> AgStrikeWeightLengths { get; set; }
        public double AgStikeWeightRequirement_Required { get; set; }
        public double AgStikeWeightRequirement_TgtRequired { get; set; }
        public double AgStikeWeightRequirement_SareRequired { get; set; }
        public double AgStikeWeightRequirement_Supplied { get; set; }
    }
}
