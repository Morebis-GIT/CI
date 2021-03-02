using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    public class AgTimeSlice : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgDayPartId { get; set; }

        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public int DayPartNo { get; set; }
        public string StartDate { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
