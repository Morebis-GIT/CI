using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions
{
    public static class CampaignMappingExtensions
    {
        private const string IgnoreCollectionsItemName = "ignoreCollections";

        public static IMappingOperationOptions<Campaign, Entities.Tenant.Campaigns.Campaign> IgnoreCollections(
            this IMappingOperationOptions<Campaign, Entities.Tenant.Campaigns.Campaign> opts)
        {
            if (opts == null)
            {
                return null;
            }

            if (!opts.Items.ContainsKey(IgnoreCollectionsItemName))
            {
                opts.Items.Add(IgnoreCollectionsItemName, true);
            }

            return opts;
        }

        public static bool AreCollectionsIgnored(this IMappingOperationOptions opts)
        {
            return opts?.Items.ContainsKey(IgnoreCollectionsItemName) ?? false;
        }
    }
}
