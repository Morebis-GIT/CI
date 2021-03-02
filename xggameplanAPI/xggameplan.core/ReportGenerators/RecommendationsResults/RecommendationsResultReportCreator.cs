using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignResults
{
    /// <summary>
    /// Recommendation result report creator.
    /// </summary>
    /// <seealso cref="xggameplan.core.ReportGenerators.ScenarioCampaignResults.RecommendationsResultReportCreatorBase" />
    public class RecommendationsResultReportCreator : RecommendationsResultReportCreatorBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private IDictionary<string, Clash> _clashes;
        private IDictionary<string, Product> _products;
        private IDictionary<string, Campaign> _campaigns;
        private IDictionary<string, Demographic> _demographics;
        private IDictionary<string, SalesArea> _salesAreas;
        private TenantSettings _tenantSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationsResultReportCreator" /> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="mapper">The mapper.</param>
        public RecommendationsResultReportCreator(
            IRepositoryFactory repositoryFactory,
            IMapper mapper)
            : base(mapper)
        {
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>Prepares the related data.</summary>
        /// <param name="source">The source.</param>
        protected override void PrepareRelatedData(IReadOnlyCollection<Recommendation> source)
        {
            using (var innerScope = _repositoryFactory.BeginRepositoryScope())
            {
                var campaignsRepository = innerScope.CreateRepository<ICampaignRepository>();
                var demographicRepository = innerScope.CreateRepository<IDemographicRepository>();
                var productRepository = innerScope.CreateRepository<IProductRepository>();
                var clashRepository = innerScope.CreateRepository<IClashRepository>();
                var salesAreaRepository = innerScope.CreateRepository<ISalesAreaRepository>();
                var tenantSettingsRepository = innerScope.CreateRepository<ITenantSettingsRepository>();

                var campaignIds = new HashSet<string>();
                var productRefs = new HashSet<string>();
                var demographicRefs = new HashSet<string>();

                foreach (var recommendation in source)
                {
                    _ = campaignIds.Add(recommendation.ExternalCampaignNumber);
                    _ = productRefs.Add(recommendation.Product);
                    _ = demographicRefs.Add(recommendation.Demographic);
                }

                _campaigns = campaignsRepository.FindByRefs(campaignIds.ToList()).ToDictionary(x => x.ExternalId);
                _products = productRepository.FindByExternal(productRefs.ToList()).ToDictionary(x => x.Externalidentifier);
                _demographics = demographicRepository.GetByExternalRef(demographicRefs.ToList())
                    .ToDictionary(x => x.ExternalRef);
                _clashes = clashRepository.GetAll().ToDictionary(r => r.Externalref);
                _salesAreas = salesAreaRepository.GetAll().ToDictionary(x => x.Name);
                _tenantSettings = tenantSettingsRepository.Get();
            }
        }

        /// <summary> Resolves the tenant settings. </summary>
        /// <returns></returns>
        protected override TenantSettings ResolveTenantSettings() => _tenantSettings;

        /// <summary>Resolves the campaign.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Campaign ResolveCampaign(Recommendation item)
        {
            if (!string.IsNullOrWhiteSpace(item.ExternalCampaignNumber))
            {
                _ = _campaigns.TryGetValue(item.ExternalCampaignNumber, out var campaign);
                return campaign;
            }

            return null;
        }

        /// <summary>Resolves the sales area.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override SalesArea ResolveSalesArea(Recommendation item)
        {
            if (!string.IsNullOrWhiteSpace(item.SalesArea))
            {
                _ = _salesAreas.TryGetValue(item.SalesArea, out var salesArea);
                return salesArea;
            }

            return null;
        }

        /// <summary>Resolves the demographic.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Demographic ResolveDemographic(Recommendation item)
        {
            if (!string.IsNullOrWhiteSpace(item.Demographic))
            {
                _ = _demographics.TryGetValue(item.Demographic, out var demographic);
                return demographic;
            }

            return null;
        }

        /// <summary>Resolves the product.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Product ResolveProduct(Recommendation item)
        {
            if (!string.IsNullOrWhiteSpace(item.Product))
            {
                _ = _products.TryGetValue(item.Product, out var product);
                return product;
            }

            return null;
        }

        /// <summary>Resolves the product clash.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Clash ResolveClash(Recommendation item)
        {
            var product = ResolveProduct(item);

            if (!string.IsNullOrWhiteSpace(product?.ClashCode))
            {
                _ = _clashes.TryGetValue(product.ClashCode, out var clash);
                return clash;
            }

            return null;
        }

        /// <summary>Resolves the product parent clash.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Clash ResolveParentClash(Recommendation item)
        {
            var clash = ResolveClash(item);

            if (!string.IsNullOrWhiteSpace(clash?.ParentExternalidentifier))
            {
                _ = _clashes.TryGetValue(clash.ParentExternalidentifier, out var parentClash);
                return parentClash;
            }

            return null;
        }
    }
}
