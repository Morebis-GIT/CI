using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AgPart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgCampaignSalesAreaId { get; set; }

        public int CampaignNo { get; set; }
        public string StartDate { get; set; }
        public int SalesAreaNo { get; set; }
        public int DayPartNo { get; set; }        
        public double AgPartRequirement_Required { get; set; }
        public double AgPartRequirement_TgtRequired { get; set; }
        public double AgPartRequirement_SareRequired { get; set; }
        public double AgPartRequirement_Supplied { get; set; }
    }
}
