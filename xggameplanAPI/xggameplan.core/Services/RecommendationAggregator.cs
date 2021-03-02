using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using xggameplan.core.Interfaces;
using xggameplan.Model;

namespace xggameplan.core.Services
{
    public class RecommendationAggregator : IRecommendationAggregator
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IProductRepository _productRepository;

        public RecommendationAggregator(
            IRecommendationRepository recommendationRepository,
            ICampaignRepository campaignRepository,
            IProductRepository productRepository)
        {
            _recommendationRepository = recommendationRepository;
            _campaignRepository = campaignRepository;
            _productRepository = productRepository;
        }

        public IReadOnlyCollection<RecommendationAggregateModel> Aggregate(Guid scenarioId)
        {
            // We don't need action B"A" i.e., smooth recommendations
            var scenario = _recommendationRepository.GetCampaigns(scenarioId)
                .Where(r => !r.Action.Equals("A", StringComparison.OrdinalIgnoreCase)).ToList();

            // Group based on ExternalCampaignNumber
            var recommendationsAggregate = scenario
                .GroupBy(r => r.ExternalCampaignNumber).Select(g => new RecommendationAggregateModel()
                {
                    ExternalCampaignNumber = g.Key,
                    SpotRating = g.Where(_ => _.Action.Equals("B", StringComparison.OrdinalIgnoreCase))
                                     .Sum(_ => _.SpotRating) -
                                 g.Where(_ => _.Action.Equals("C", StringComparison.OrdinalIgnoreCase))
                                     .Sum(_ => _.SpotRating),
                    CampaignGroup = null,
                    CampaignName = null,
                }).ToList();

            // Populate the campaign list
            var campaignIds = recommendationsAggregate.Select(r => r.ExternalCampaignNumber).ToList();
            var campaigns = campaignIds.Any()
                ? _campaignRepository.FindByRefs(campaignIds).ToList()
                : new List<Campaign>();

            // Populate the corresponding product list
            var productRefs = campaigns
                .Where(c => !string.IsNullOrEmpty(c.Product))
                .Select(c => c.Product)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var products =
                productRefs.Count == 0 ? new List<Product>() : _productRepository.FindByExternal(productRefs).ToList();

            // Populate the campaign details & advertiser name
            foreach (var recAgg in recommendationsAggregate)
            {
                var camp = campaigns.FirstOrDefault(c => c.ExternalId == recAgg.ExternalCampaignNumber);

                if (camp is null)
                {
                    continue;
                }

                recAgg.CampaignGroup = camp.CampaignGroup;
                recAgg.CampaignName = camp.Name;
                recAgg.TargetRatings = camp.TargetRatings;
                recAgg.ActualRatings = camp.ActualRatings;
                recAgg.EndDateTime = camp.EndDateTime;
                recAgg.IsPercentage = camp.IsPercentage;
                recAgg.AdvertiserName = !string.IsNullOrEmpty(camp.Product)
                    ? products.FirstOrDefault(p =>
                            p.Externalidentifier.Equals(camp.Product, StringComparison.OrdinalIgnoreCase))
                        ?.AdvertiserName
                    : null;
            }

            return recommendationsAggregate;
        }
    }
}
