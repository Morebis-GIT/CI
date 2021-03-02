using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class CampaignSettings_ById
        : AbstractIndexCreationTask<CampaignSettings>
    {
        public static string DefaultIndexName => "CampaignSettings/ById";

        public CampaignSettings_ById()
        {
            Map = campaignSettings =>
                from campaignSetting in campaignSettings
                select new
                {
                    campaignSetting.Id
                };
        }
    }
}
