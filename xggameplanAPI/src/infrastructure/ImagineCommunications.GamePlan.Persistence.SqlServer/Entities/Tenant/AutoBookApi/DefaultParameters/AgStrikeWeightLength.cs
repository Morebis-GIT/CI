using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AgStrikeWeightLength : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgStrikeWeightId { get; set; }

        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public int SpotLength { get; set; }
        public int MultiPartNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double AgStrikeWeightLengthRequirement_Required { get; set; }
        public double AgStrikeWeightLengthRequirement_TgtRequired { get; set; }
        public double AgStrikeWeightLengthRequirement_SareRequired { get; set; }        
        public double AgStrikeWeightLengthRequirement_Supplied { get; set; }
    }
}
