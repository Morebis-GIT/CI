using System;
using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace xggameplan.Model
{
    public class CampaignRunProcessesSettingsModel
    {
        public string ExternalId { get; set; }
        public bool? InefficientSpotRemoval { get; set; }
        public IncludeRightSizer? IncludeRightSizer { get; set; }
        public int DeliveryCappingGroupId { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ExternalId))
            {
                throw new ArgumentException("Campaign External Id cannot be empty");
            }

            if (!InefficientSpotRemoval.HasValue && !IncludeRightSizer.HasValue && DeliveryCappingGroupId == 0)
            {
                throw new ArgumentException("Pointless process settings. Neither ISR, RS nor delivery capping group id values provided");
            }
        }
    }
}
