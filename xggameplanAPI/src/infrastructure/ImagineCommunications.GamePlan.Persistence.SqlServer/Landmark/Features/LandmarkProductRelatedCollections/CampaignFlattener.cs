using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.core.Services.CampaignFlattening;
using CampaignEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.Campaign;
using Clash = ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects.Clash;
using ClashEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Clash;
using Demographic = ImagineCommunications.GamePlan.Domain.Shared.Demographics.Demographic;
using DemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Demographic;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Gets the flattened view of all the campaigns taking into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    public class CampaignFlattener : CampaignFlattenerBase
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByNullableIdCacheAccessor _salesAreaByIdCache;
        private readonly IMapper _mapper;

        private IDictionary<Guid, ProductLink> _productLinks;

        /// <summary>Initializes a new instance of the <see cref="CampaignFlattener" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="mapper">The mapper.</param>
        public CampaignFlattener(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByNullableIdCacheAccessor salesAreaByIdCache,
            IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _mapper = mapper;
        }

        /// <summary>Prepares the related campaign data.</summary>
        /// <param name="campaigns">The campaigns.</param>
        protected override void PrepareRelatedCampaignData(IReadOnlyCollection<Campaign> campaigns)
        {
            _productLinks =
                (from campaignWithProductRelations in _dbContext.Specific.View<CampaignWithProductRelations>()
                 join campaign in _dbContext.Query<CampaignEntity>() on campaignWithProductRelations.CampaignId
                     equals campaign.Id
                 join demographicJoin in _dbContext.Query<DemographicEntity>() on campaign.Demographic equals
                     demographicJoin.ExternalRef into dJoin
                 from demographic in dJoin.DefaultIfEmpty()
                 join productJoin in _dbContext.Query<ProductEntity>() on campaignWithProductRelations.ProductId
                     equals productJoin.Id into pJoin
                 from product in pJoin.DefaultIfEmpty()
                 join clashJoin in _dbContext.Query<ClashEntity>() on product.ClashCode equals clashJoin.Externalref
                     into clJoin
                 from clash in clJoin.DefaultIfEmpty()
                 join parentClashJoin in _dbContext.Query<ClashEntity>() on clash.ParentExternalidentifier equals
                     parentClashJoin.Externalref into pclJoin
                 from parentClash in pclJoin.DefaultIfEmpty()
                 join advertiserJoin in _dbContext.Query<Advertiser>() on campaignWithProductRelations.AdvertiserId
                     equals advertiserJoin.Id into adJoin
                 from advertiser in adJoin.DefaultIfEmpty()
                 join agencyJoin in _dbContext.Query<Agency>() on campaignWithProductRelations.AgencyId equals
                     agencyJoin.Id into agJoin
                 from agency in agJoin.DefaultIfEmpty()
                 select new ProductLink
                 {
                     CampaignId = campaignWithProductRelations.CampaignId,
                     Demographic = demographic,
                     Clash = clash,
                     ParentClash = parentClash,
                     Product = product,
                     Advertiser = advertiser,
                     Agency = agency
                 })
                .AsNoTracking()
                .ToDictionary(k => k.CampaignId);
        }

        /// <summary>Resolves the demographic.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Demographic ResolveDemographic(Campaign campaign)
        {
            var productLinkExists = _productLinks.TryGetValue(campaign.Id, out var productLink);
            if (productLinkExists && !(productLink.Demographic is null))
            {
                return _mapper.Map<Demographic>(productLink.Demographic);
            }

            return null;
        }

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Product ResolveProduct(Campaign campaign)
        {
            var productLinkExists = _productLinks.TryGetValue(campaign.Id, out var productLink);
            if (productLinkExists && !(productLink.Product is null))
            {
                return new Product
                {
                    Name = productLink.Product.Name,
                    AdvertiserName = productLink.Advertiser?.Name,
                    AgencyName = productLink.Agency?.Name
                };
            }

            return null;
        }

        /// <summary>Resolves the product clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Clash ResolveClash(Campaign campaign)
        {
            var productLinkExists = _productLinks.TryGetValue(campaign.Id, out var productLink);
            if (productLinkExists && !(productLink.Clash is null))
            {
                return _mapper.Map<Clash>(productLink.Clash, opts => opts.UseEntityCache(_salesAreaByIdCache));
            }

            return null;
        }

        /// <summary>Resolve the parent product clash</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Clash ResolveParentClash(Campaign campaign)
        {
            var productLinkExists = _productLinks.TryGetValue(campaign.Id, out var productLink);
            if (productLinkExists && !(productLink.ParentClash is null))
            {
                return _mapper.Map<Clash>(productLink.ParentClash, opts => opts.UseEntityCache(_salesAreaByIdCache));
            }

            return null;
        }

        private class ProductLink
        {
            public Guid CampaignId { get; set; }
            public DemographicEntity Demographic { get; set; }
            public ProductEntity Product { get; set; }
            public ClashEntity Clash { get; set; }
            public ClashEntity ParentClash { get; set; }
            public Advertiser Advertiser { get; set; }
            public Agency Agency { get; set; }
        }
    }
}
