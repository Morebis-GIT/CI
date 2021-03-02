using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.core.ReportGenerators.ScenarioCampaignResults;
using AgencyEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Agency;
using CampaignEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.Campaign;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Recommendation result report creator which takes into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    /// <seealso cref="xggameplan.core.ReportGenerators.ScenarioCampaignResults.RecommendationsResultReportCreatorBase" />
    public class RecommendationsResultReportCreator : RecommendationsResultReportCreatorBase
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;

        private IDictionary<string, SalesArea> _salesAreas;
        private IDictionary<string, Demographic> _demographics;
        private IDictionary<string, Clash> _clashes;
        private IDictionary<string, ProductLinkDomain> _productLinks;
        private TenantSettings _tenantSettings;

        /// <summary>Initializes a new instance of the <see cref="RecommendationsResultReportCreator" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tenantSettingsRepository">The tenant settings repository.</param>
        /// <param name="demographicRepository">The demographic repository.</param>
        /// <param name="salesAreaRepository">The sales area repository.</param>
        /// <param name="clashRepository">The clash repository.</param>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="mapper">The mapper.</param>
        public RecommendationsResultReportCreator(
            ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            IRepositoryFactory repositoryFactory,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            IMapper mapper)
            : base(mapper)
        {
            _dbContextFactory = dbContextFactory;
            _repositoryFactory = repositoryFactory;
            _salesAreaByIdCache = salesAreaByIdCache;
            _mapper = mapper;
        }

        /// <summary>Prepares the related data.</summary>
        /// <param name="source">The source.</param>
        protected override void PrepareRelatedData(IReadOnlyCollection<Recommendation> source)
        {
            var campaignExternalIds = source
                .Select(x => x.ExternalCampaignNumber)
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x));

            using (var repositoryScope = _repositoryFactory.BeginRepositoryScope())
            using (var dbContext = _dbContextFactory.Create())
            {
                var salesAreaRepository = repositoryScope.CreateRepository<ISalesAreaRepository>();
                var demographicRepository = repositoryScope.CreateRepository<IDemographicRepository>();
                var clashRepository = repositoryScope.CreateRepository<IClashRepository>();
                var productRepository = repositoryScope.CreateRepository<IProductRepository>();

                _salesAreas = salesAreaRepository.GetAll().ToDictionary(x => x.Name);
                _demographics = demographicRepository.GetAll().ToDictionary(x => x.ExternalRef);
                _clashes = clashRepository.GetAll().ToDictionary(x => x.Externalref);

                _productLinks =
                (from campaignWithProductRelations in dbContext.Specific.View<CampaignWithProductRelations>()
                 join campaign in dbContext.Query<CampaignEntity>()
                         .Include(x => x.SalesAreaCampaignTargets)
                         .ThenInclude(x => x.SalesAreaGroup)
                     on campaignWithProductRelations.CampaignId equals campaign.Id
                 join productJoin in dbContext.Query<ProductEntity>() on campaignWithProductRelations.ProductId equals productJoin.Uid into pJoin
                 from product in pJoin.DefaultIfEmpty()
                 join agencyJoin in dbContext.Query<AgencyEntity>() on campaignWithProductRelations.AgencyId equals agencyJoin.Id into agJoin
                 from agency in agJoin.DefaultIfEmpty()
                 where campaignExternalIds.Contains(campaign.ExternalId)
                 select new ProductLinkSql
                 {
                     Campaign = campaign,
                     Product = product,
                     Agency = agency
                 })
                .AsNoTracking()
                .ToDictionary(x => x.Campaign.ExternalId, x => new ProductLinkDomain
                {
                    Campaign = _mapper.Map<Campaign>(x.Campaign, opts => opts.UseEntityCache(_salesAreaByIdCache)),
                    Product = x.Product is null ? null : new Product
                    {
                        Externalidentifier = x.Product.Externalidentifier,
                        Name = x.Product.Name,
                        AgencyName = x.Agency?.Name,
                        ClashCode = x.Product.ClashCode
                    }
                });

                foreach (var item in source)
                {
                    if (!_productLinks.TryGetValue(item.ExternalCampaignNumber, out var productLink) ||
                        productLink.Product?.Externalidentifier == item.Product ||
                        productLink.CampaignProductLoadAttempted ||
                        !productLink.Campaign.CreationDate.HasValue)
                    {
                        continue;
                    }

                    productLink.CampaignProduct = productRepository.FindByExternal(item.Product, productLink.Campaign.CreationDate.Value).FirstOrDefault();
                    productLink.CampaignProductLoadAttempted = true;
                }
            }
        }

        /// <summary> Resolves the tenant settings. </summary>
        /// <returns></returns>
        protected override TenantSettings ResolveTenantSettings() => _tenantSettings;

        /// <summary>Resolves the campaign.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Campaign ResolveCampaign(Recommendation item) =>
            !string.IsNullOrWhiteSpace(item.ExternalCampaignNumber) &&
            _productLinks.TryGetValue(item.ExternalCampaignNumber, out var productLink)
                ? productLink.Campaign
                : null;

        /// <summary>Resolves the sales area.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override SalesArea ResolveSalesArea(Recommendation item) =>
            !string.IsNullOrWhiteSpace(item.SalesArea) &&
            _salesAreas.TryGetValue(item.SalesArea, out var salesArea)
                ? salesArea
                : null;

        /// <summary>Resolves the demographic.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Demographic ResolveDemographic(Recommendation item) =>
            !string.IsNullOrWhiteSpace(item.Demographic) &&
            _demographics.TryGetValue(item.Demographic, out var demographic)
                ? demographic
                : null;

        /// <summary>Resolves the product.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Product ResolveProduct(Recommendation item)
        {
            var productLinkExists = _productLinks.TryGetValue(item.ExternalCampaignNumber, out var productLink);

            if (!productLinkExists)
            {
                return null;
            }

            if (!(productLink.Product is null) && productLink.Product.Externalidentifier == item.Product)
            {
                return productLink.Product;
            }

            return productLink.CampaignProduct;
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

        private class ProductLinkSql
        {
            public CampaignEntity Campaign { get; set; }
            public ProductEntity Product { get; set; }
            public AgencyEntity Agency { get; set; }
        }

        private class ProductLinkDomain
        {
            public Campaign Campaign { get; set; }
            public Product Product { get; set; }
            public Product CampaignProduct { get; set; }
            public bool CampaignProductLoadAttempted { get; set; }
        }
    }
}
