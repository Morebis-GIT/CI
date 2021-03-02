using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using Raven.Client.Indexes;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Campaigns_ById
        : AbstractIndexCreationTask<Campaign>
    {
        public static string DefaultIndexName => "Campaigns/ById";

        public Campaigns_ById()
        {
            Map = campaigns =>
                from campaign in campaigns
                select new
                {
                    campaign.Id
                };
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
