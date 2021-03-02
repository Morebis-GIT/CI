using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Projections;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.core.ReportGenerators.DataSnapshotRequirements;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignResults
{
    public class ReducedRecommendationsResultsDataSnapshot : ICampaignsHolder, IClashesHolder, IDemographicsHolder, IProductsHolder, ISalesAreasHolder, ITenantSettingsHolder
    {
        public TenantSettings Settings { get; set; }
        public IEnumerable<Clash> Clashes { get; set; } = Enumerable.Empty<Clash>();
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
        public IEnumerable<IReducedCampaign> Campaigns { get; set; } = Enumerable.Empty<Campaign>();
        public IEnumerable<SalesArea> SalesAreas { get; set; } = Enumerable.Empty<SalesArea>();
        public IEnumerable<Demographic> Demographics { get; set; } = Enumerable.Empty<Demographic>();
    }
}
