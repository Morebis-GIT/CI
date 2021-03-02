using System;
using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects
{
    public class ScenarioCampaignExtendedResultItem : ScenarioCampaignResultItem
    {
        public string MediaSalesGroup { get; set; }

        public string ProductAssignee { get; set; }

        public bool StopBooking { get; set; }

        public DateTime? CreationDate { get; set; }

        public bool? AutomatedBooked { get; set; }

        public TopTail? TopTail { get; set; }

        public string ReportingCategory { get; set; }

        public string ClashCode { get; set; }

        public string AgencyName { get; set; }
    }
}
