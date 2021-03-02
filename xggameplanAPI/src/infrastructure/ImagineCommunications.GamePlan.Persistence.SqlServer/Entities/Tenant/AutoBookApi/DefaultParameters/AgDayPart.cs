using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public class AgDayPart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgCampaignSalesAreaId { get; set; }

        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public int DayPartNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }        
        public double AgDayPartRequirement_Required { get; set; }
        public double AgDayPartRequirement_TgtRequired { get; set; }
        public double AgDayPartRequirement_SareRequired { get; set; }
        public double AgDayPartRequirement_Supplied { get; set; }
        public int NbrAgTimeSlices { get; set; }
        public int SpotMaxRatings { get; set; }
        public List<AgTimeSlice> AgTimeSlices { get; set; }
        public int NbrAgDayPartLengths { get; set; }
        public List<AgDayPartLength> AgDayPartLengths { get; set; }
    }
}
