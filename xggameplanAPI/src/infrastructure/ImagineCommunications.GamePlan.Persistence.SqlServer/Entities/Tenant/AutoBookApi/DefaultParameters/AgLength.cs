using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AgLength : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgCampaignSalesAreaId { get; set; }

        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public int SpotLength { get; set; }
        public int MultiPartNo { get; set; }
        public int NoOfSpots { get; set; }
        public double PriceFactor { get; set; }        
        public double AgLengthRequirement_Required { get; set; }
        public double AgLengthRequirement_TgtRequired { get; set; }
        public double AgLengthRequirement_SareRequired { get; set; }
        public double AgLengthRequirement_Supplied { get; set; }
        public int NbrAgMultiParts { get; set; }
        public List<AgMultiPart> AgMultiParts { get; set; }
    }
}
