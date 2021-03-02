using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using xggameplan.common.Utilities;
using xggameplan.core.ReportGenerators.ScenarioCampaignFailures;
using xggameplan.core.ReportGenerators.ScenarioCampaignResults;

namespace xggameplan.Controllers
{
    public partial class ScenarioResultsController
    {
        public (ReducedRecommendationsResultsDataSnapshot snapshot, IEnumerable<RecommendationReducedModel> data) GenerateReducedRecommendationsResultsDataShapshot(
            Guid scenarioId,
            List<string> externalCampaignNumbers,
            List<string> salesAreaNames)
        {
            IEnumerable<Recommendation> recommendations = FilterRecommendations(scenarioId, externalCampaignNumbers, salesAreaNames);

            if ((recommendations is null) || !recommendations.Any())
            {
                return (null, null);
            }

            var reducedItems = _mapper.Map<List<RecommendationReducedModel>>(recommendations);

            var campaignIds = new HashSet<string>();
            foreach (var recommendation in reducedItems)
            {
                campaignIds.Add(recommendation.ExternalCampaignNumber);
            }

            var snapshot = new ReducedRecommendationsResultsDataSnapshot
            {
                SalesAreas = _salesAreaRepository.GetAll(),
                Settings = _tenantSettingsRepository.Get(),
                Clashes = _clashRepository.GetAll(),
                Campaigns = _campaignRepository.FindByRefs(campaignIds.ToList())
            };

            return (snapshot, reducedItems);
        }

        private IEnumerable<Recommendation> FilterRecommendations(
            Guid scenarioId,
            List<string> externalCampaignNumbers,
            List<string> salesAreaNames)
        {
            var filteredRecommendations = new List<Recommendation>();
            var recommendations = _recommendationRepository.GetByScenarioId(scenarioId);

            if (!externalCampaignNumbers.Any() && !salesAreaNames.Any())
            {
                return recommendations;
            }

            if (externalCampaignNumbers.Any() && salesAreaNames.Any())
            {
                filteredRecommendations = recommendations.Where(x => externalCampaignNumbers.Contains(x.ExternalCampaignNumber) && salesAreaNames.Contains(x.SalesArea)).ToList();
            }
            else if (externalCampaignNumbers.Any())
            {
                filteredRecommendations = recommendations.Where(x => externalCampaignNumbers.Contains(x.ExternalCampaignNumber)).ToList();
            }
            else if (salesAreaNames.Any())
            {
                filteredRecommendations = recommendations.Where(x => salesAreaNames.Contains(x.SalesArea)).ToList();
            }

            return filteredRecommendations;
        }

        public (ScenarioCampaignFailuresDataSnapshot snapshot, IReadOnlyCollection<ScenarioCampaignFailure> data) GenerateScenarioCampaignFailuresDataShapshot(Guid scenarioId)
        {
            var failures = _scenarioCampaignFailureRepository.FindByScenarioId(scenarioId).ToList();

            if (failures.Count == 0)
            {
                return (null, null);
            }

            var failureTypeIds = failures
                .Select(r => r.FailureType)
                .Distinct();

            var campaignIds = failures
                .Select(r => r.ExternalCampaignId)
                .Distinct();

            var snapshot = new ScenarioCampaignFailuresDataSnapshot();
            snapshot.Campaigns = _campaignRepository.FindByRefs(campaignIds.ToList());
            snapshot.FaultTypes = _functionalAreaRepository.FindFaultTypes(failureTypeIds.ToList());

            return (snapshot, failures);
        }

        public List<ScenarioCampaignFailureModel> ExtendScenarioCampaignFailureModel(List<ScenarioCampaignFailure> scenarioCampaignFailures)
        {
            var scenarioCampaignFailureModelList = new List<ScenarioCampaignFailureModel>();
            if (scenarioCampaignFailures is null || !scenarioCampaignFailures.Any())
            {
                return scenarioCampaignFailureModelList;
            }
            var campaigns = new Dictionary<string, Campaign>();
            var failureTypes = new Dictionary<int, FaultType>();

            var failureTypeIds = scenarioCampaignFailures
                .Select(r => r.FailureType)
                .Distinct();
            var campaignIds = scenarioCampaignFailures
                .Select(r => r.ExternalCampaignId)
                .Distinct();

            var tenantStartDayOfWeek = _tenantSettingsRepository.GetStartDayOfWeek();
            var weekDays = DaypartDayFormattingUtilities.GetWeekDaysWithCustomStart(tenantStartDayOfWeek);

            failureTypes = _functionalAreaRepository
                .FindFaultTypes(failureTypeIds.ToList())
                .ToDictionary(r => r.Id);
            campaigns = _campaignRepository
                .FindByRefs(campaignIds.ToList())
                .ToDictionary(r => r.ExternalId);

            foreach (var item in scenarioCampaignFailures)
            {
                var scenarioCampaignResultModel = new ScenarioCampaignFailureModel()
                {
                    Id = item.Id,
                    ExternalCampaignId = item.ExternalCampaignId,
                    SalesAreaGroupName = item.SalesAreaGroup,
                    SalesAreaName = item.SalesArea,
                    DurationSecs = (int)item.Length.ToTimeSpan().TotalSeconds,
                    MultipartNo = item.MultipartNo,
                    StrikeWeightStartDate = item.StrikeWeightStartDate.ToUniversalTime(),
                    StrikeWeightEndDate = item.StrikeWeightEndDate.ToUniversalTime(),
                    DayPartStartTime = item.DayPartStartTime,
                    DayPartEndTime = item.DayPartEndTime,
                    DayPartDays = DaypartDayFormattingUtilities.FormatWeekDays(weekDays, item.DayPartDays, tenantStartDayOfWeek),
                    FailureType = item.FailureType,
                    FailureCount = item.FailureCount,
                    PassesEncounteringFailure = item.PassesEncounteringFailure
                };
                if (item.ExternalCampaignId != null
                    && campaigns.TryGetValue(item.ExternalCampaignId, out Campaign campaign))
                {
                    scenarioCampaignResultModel.CampaignName = campaign.Name;
                }
                if (item.FailureType != null
                    && failureTypes.TryGetValue(item.FailureType, out FaultType faultType))
                {
                    scenarioCampaignResultModel.FailureTypeName
                        = faultType.Description?.FirstOrDefault(e => e.Key == "ENG").Value;
                }
                scenarioCampaignFailureModelList.Add(scenarioCampaignResultModel);
            }
            return scenarioCampaignFailureModelList;
        }

        private void SetRecommendationProductInfo(ref IEnumerable<RecommendationSimple> recommendationsSimples)
        {
            var productExternalIds = recommendationsSimples.Select(r => r.Product).Distinct();
            var productsDictionary = _productRepository
                                     .FindByExternal(productExternalIds.ToList())
                                     .ToDictionary(r => r.Externalidentifier);

            foreach (var recommendation in recommendationsSimples)
            {
                if (recommendation.Product != null &&
                    productsDictionary.TryGetValue(recommendation.Product, out Product product))
                {
                    recommendation.AgencyName = product.AgencyName;
                    recommendation.ProductName = product.Name;
                }
            }
        }
    }
}
