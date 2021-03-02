using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.core.Interfaces;
using xggameplan.Model;

namespace xggameplan.core.Services
{
    public class SpotModelCreator : ISpotModelCreator
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IProductRepository _productRepository;
        private readonly IClashRepository _clashRepository;
        private readonly IMapper _mapper;

        public SpotModelCreator(
            ICampaignRepository campaignRepository,
            IProductRepository productRepository,
            IClashRepository clashRepository,
            IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _productRepository = productRepository;
            _clashRepository = clashRepository;
            _mapper = mapper;
        }

        public IEnumerable<SpotModel> Create(IReadOnlyCollection<Spot> spots)
        {
            if (spots is null || spots.Count == 0)
            {
                return Enumerable.Empty<SpotModel>();
            }

            var clashes = new List<Clash>();

            var campaignExternalIds = spots.Select(s => s.ExternalCampaignNumber)
                .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var campaigns = _campaignRepository.FindByRefs(campaignExternalIds).ToList();

            var productExternalIds = spots.Select(s => s.Product).Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var products = _productRepository.FindByExternal(productExternalIds).ToList();

            if (products.Count != 0)
            {
                var clashExternalIds = products.Select(p => p.ClashCode).Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                clashes = _clashRepository.FindByExternal(clashExternalIds).ToList();
            }

            return spots.Select(spot =>
            {
                var campaign =
                    campaigns.FirstOrDefault(c => c.ExternalId.Equals(spot.ExternalCampaignNumber,
                        StringComparison.OrdinalIgnoreCase));
                var product = products.FirstOrDefault(
                    p => p.Externalidentifier.Equals(spot.Product, StringComparison.OrdinalIgnoreCase));

                var clashList = product != null
                    ? clashes.Where(
                        c => c.Externalref.Equals(product.ClashCode, StringComparison.OrdinalIgnoreCase)).ToList()
                    : null;
                return _mapper.Map<SpotModel>(Tuple.Create(spot, campaign, product, clashList));
            });
        }
    }
}
