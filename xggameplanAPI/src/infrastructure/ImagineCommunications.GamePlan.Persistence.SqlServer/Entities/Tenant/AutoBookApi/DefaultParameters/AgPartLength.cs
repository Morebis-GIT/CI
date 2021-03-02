using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AgPartLength : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgCampaignSalesAreaId { get; set; }

        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public string StartDate { get; set; }
        public int DayPartType { get; set; }
        public int DayPartNo { get; set; }
        public long SpotLength { get; set; }
        public int MultipartNumber { get; set; }
        public double AgPartLengthRequirement_Required { get; set; }
        public double AgPartLengthRequirement_TgtRequired { get; set; }
        public double AgPartLengthRequirement_SareRequired { get; set; }        
        public double AgPartLengthRequirement_Supplied { get; set; }
    }
}
