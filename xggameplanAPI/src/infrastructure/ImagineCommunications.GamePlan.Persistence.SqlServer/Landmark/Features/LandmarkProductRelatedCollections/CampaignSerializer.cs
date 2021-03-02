using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Campaigns;
using AdvertiserEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Advertiser;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Serializes campaign collection into xml file taking into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    public class CampaignSerializer : CampaignSerializerBase
    {
        private readonly ISqlServerTenantDbContext _dbContext;

        private IDictionary<Guid, ProductLink> _productLinks;

        /// <summary>Initializes a new instance of the <see cref="CampaignSerializer" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="auditEventRepository">The audit event repository.</param>
        /// <param name="featureManager">The feature manager.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="mapper">The mapper.</param>
        public CampaignSerializer(
            ISqlServerTenantDbContext dbContext,
            IAuditEventRepository auditEventRepository,
            IFeatureManager featureManager,
            IClock clock,
            IMapper mapper)
            : base(auditEventRepository, featureManager, clock, mapper)
        {
            _dbContext = dbContext;
        }

        /// <summary>Prepares the related data.</summary>
        /// <param name="campaigns">The campaigns.</param>
        protected override void PrepareRelatedData(IReadOnlyCollection<Campaign> campaigns)
        {
            var campaignIds = campaigns.Select(x => x.Id);

            _productLinks =
                (from campaignWithProductRelations in _dbContext.Specific.View<CampaignWithProductRelations>()
                 join productJoin in _dbContext.Query<ProductEntity>() on campaignWithProductRelations.ProductId
                     equals productJoin.Uid into pJoin
                 from product in pJoin.DefaultIfEmpty()
                 join clashRootJoin in _dbContext.Specific.View<ClashRoot>() on product.ClashCode
                     equals clashRootJoin.ExternalRef into crJoin
                 from clashRoot in crJoin.DefaultIfEmpty()
                 join advertiserJoin in _dbContext.Query<AdvertiserEntity>() on campaignWithProductRelations
                     .AdvertiserId equals advertiserJoin.Id into adJoin
                 from advertiser in adJoin.DefaultIfEmpty()
                 where campaignIds.Contains(campaignWithProductRelations.CampaignId)
                 select new ProductLink
                 {
                     CampaignId = campaignWithProductRelations.CampaignId,
                     Product = product,
                     Advertiser = advertiser,
                     ClashRootExternalRef = clashRoot.RootExternalRef
                 })
                .AsNoTracking()
                .ToDictionary(k => k.CampaignId);
        }

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Product ResolveProduct(Campaign campaign)
        {
            var productLinksExists = _productLinks.TryGetValue(campaign.Id, out var productLink);

            if (productLinksExists && !(productLink.Product is null))
            {
                return new Product
                {
                    Uid = productLink.Product.Uid,
                    ClashCode = productLink.Product.ClashCode,
                    AdvertiserIdentifier = productLink.Advertiser?.ExternalIdentifier ?? string.Empty
                };
            }

            return null;
        }

        /// <summary>
        /// Resolves root clash code for the product clash.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override string ResolveRootClashCode(Campaign campaign)
        {
            _ = _productLinks.TryGetValue(campaign.Id, out var productLink);
            return productLink?.ClashRootExternalRef;
        }

        protected override TimeSpan AdjustAgTimesliceToTime(TimeSpan value) => value;

        private class ProductLink
        {
            public Guid CampaignId { get; set; }
            public ProductEntity Product { get; set; }
            public AdvertiserEntity Advertiser { get; set; }
            public string ClashRootExternalRef { get; set; }
        }
    }
}
