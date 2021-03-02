using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using xggameplan.core.Services.CampaignFlattening;

namespace xggameplan.core.Services
{
    public class CampaignFlattener : CampaignFlattenerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly IClashRepository _clashRepository;

        private Dictionary<string, Clash> _clashes;
        private Dictionary<string, Product> _products;
        private Dictionary<string, Demographic> _demographics;

        public CampaignFlattener(
            IProductRepository productRepository,
            IDemographicRepository demographicRepository,
            IClashRepository clashRepository,
            IMapper mapper) : base(mapper)
        {
            _productRepository = productRepository;
            _demographicRepository = demographicRepository;
            _clashRepository = clashRepository;
        }

        protected override void PrepareRelatedCampaignData(IReadOnlyCollection<Campaign> campaigns)
        {
            var productRefs = new HashSet<string>();
            var demographicRefs = new HashSet<string>();

            foreach (var campaign in campaigns)
            {
                productRefs.Add(campaign.Product);
                demographicRefs.Add(campaign.DemoGraphic);
            }

            _products = _productRepository
                .FindByExternal(productRefs.ToList())
                .ToDictionary(r => r.Externalidentifier);

            _demographics = _demographicRepository
                .GetByExternalRef(demographicRefs.ToList())
                .ToDictionary(r => r.ExternalRef);

            _clashes = _clashRepository.GetAll()
                .ToDictionary(r => r.Externalref);
        }

        protected override Demographic ResolveDemographic(Campaign campaign)
        {
            _ = _demographics.TryGetValue(campaign.DemoGraphic, out var demographic);
            return demographic;
        }

        protected override Product ResolveProduct(Campaign campaign)
        {
            _ = _products.TryGetValue(campaign.Product, out var product);
            return product;
        }

        protected override Clash ResolveClash(Campaign campaign)
        {
            var product = ResolveProduct(campaign);
            if (product is null)
            {
                return null;
            }

            _ = _clashes.TryGetValue(product.ClashCode, out var clash);
            return clash;
        }

        protected override Clash ResolveParentClash(Campaign campaign)
        {
            var clash = ResolveClash(campaign);
            if (string.IsNullOrWhiteSpace(clash?.ParentExternalidentifier))
            {
                return null;
            }

            _ = _clashes.TryGetValue(clash.ParentExternalidentifier, out var parentClash);
            return parentClash;
        }
    }
}
