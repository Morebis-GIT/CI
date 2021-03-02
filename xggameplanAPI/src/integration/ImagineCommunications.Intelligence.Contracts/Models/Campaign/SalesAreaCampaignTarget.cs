using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class SalesAreaCampaignTarget
    {
        public SalesAreaCampaignTarget(string salesArea, bool stopBooking,  SalesAreaGroup salesAreaGroup, List<Multipart> multiparts, List<CampaignTarget> campaignTargets)
        {
            SalesArea = salesArea;
            StopBooking = stopBooking;
            SalesAreaGroup = salesAreaGroup;
            Multiparts = multiparts;
            CampaignTargets = campaignTargets;
        }
        
        public string SalesArea { get; }

        public SalesAreaGroup SalesAreaGroup { get; }

        public List<Multipart> Multiparts { get; }

        public List<CampaignTarget> CampaignTargets { get; }

        public bool StopBooking { get; }
    }
}
