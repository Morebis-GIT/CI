using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Helpers;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Campaigns
{
    /// <summary>
    /// Serializes campaign collection into xml file.
    /// </summary>
    /// <seealso cref="xggameplan.core.Services.OptimiserInputFilesSerialisers.Campaigns.CampaignSerializerBase" />
    public class CampaignSerializer : CampaignSerializerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IClashRepository _clashRepository;

        private IReadOnlyDictionary<string, Product> _products;
        private IReadOnlyDictionary<string, string> _clashRoots;

        /// <summary>Initializes a new instance of the <see cref="CampaignSerializer" /> class.</summary>
        /// <param name="auditEventRepository">The audit event repository.</param>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="clashRepository">The clash repository.</param>
        /// <param name="featureManager">The feature manager.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="mapper">The mapper.</param>
        public CampaignSerializer(
            IAuditEventRepository auditEventRepository,
            IProductRepository productRepository,
            IClashRepository clashRepository,
            IFeatureManager featureManager,
            IClock clock,
            IMapper mapper)
            : base(auditEventRepository, featureManager, clock, mapper)
        {
            _productRepository = productRepository;
            _clashRepository = clashRepository;
        }

        /// <summary>Prepares the related data.</summary>
        /// <param name="campaigns">The campaigns.</param>
        protected override void PrepareRelatedData(IReadOnlyCollection<Campaign> campaigns)
        {
            var productExternalIds = campaigns.Select(c => c.Product).Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList().ToList();

            _products = _productRepository.FindByExternal(productExternalIds).ToDictionary(x => x.Externalidentifier);

            var allClashes = Clash.IndexListByExternalRef(_clashRepository.GetAll());
            _clashRoots = ClashHelper.CalculateClashTopParents(allClashes);
        }

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Product ResolveProduct(Campaign campaign)
        {
            if (string.IsNullOrWhiteSpace(campaign.Product))
            {
                return null;
            }

            _ = _products.TryGetValue(campaign.Product, out var product);
            return product;
        }

        /// <summary>Resolves root clash code for the product clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override string ResolveRootClashCode(Campaign campaign)
        {
            var product = ResolveProduct(campaign);

            if (string.IsNullOrWhiteSpace(product?.ClashCode))
            {
                return null;
            }

            _ = _clashRoots.TryGetValue(product.ClashCode, out var clashRoot);
            return clashRoot;
        }
    }
}
