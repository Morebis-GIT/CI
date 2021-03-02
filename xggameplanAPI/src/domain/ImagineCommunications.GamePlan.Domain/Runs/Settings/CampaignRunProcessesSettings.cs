using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace ImagineCommunications.GamePlan.Domain.Runs.Settings
{
    public class CampaignRunProcessesSettings
    {
        public string ExternalId { get; set; }

        public bool? InefficientSpotRemoval { get; set; }

        public bool? IncludeRightSizer { get; set; }
        public RightSizerLevel? RightSizerLevel { get; set; }

        public int DeliveryCappingGroupId { get; set; }

        public bool IsEmpty =>
            !InefficientSpotRemoval.HasValue &&
            !IncludeRightSizer.HasValue &&
            DeliveryCappingGroupId == 0;

        public CampaignRunProcessesSettings Update(CampaignRunProcessesSettings model)
        {
            InefficientSpotRemoval = model.InefficientSpotRemoval;
            IncludeRightSizer = model.IncludeRightSizer;
            RightSizerLevel = model.RightSizerLevel;
            DeliveryCappingGroupId = model.DeliveryCappingGroupId;

            return this;
        }

        public override bool Equals(object obj) => obj is CampaignRunProcessesSettings settings && Equals(settings);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ExternalId != null ? ExternalId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ InefficientSpotRemoval.GetHashCode();
                hashCode = (hashCode * 397) ^ IncludeRightSizer.GetHashCode();
                hashCode = (hashCode * 397) ^ RightSizerLevel.GetHashCode();
                return hashCode;
            }
        }


        public bool Equals(CampaignRunProcessesSettings settings)
        {
            return ExternalId == settings.ExternalId &&
                   InefficientSpotRemoval == settings.InefficientSpotRemoval &&
                   IncludeRightSizer == settings.IncludeRightSizer &&
                   RightSizerLevel == settings.RightSizerLevel;
        }
    }
}
