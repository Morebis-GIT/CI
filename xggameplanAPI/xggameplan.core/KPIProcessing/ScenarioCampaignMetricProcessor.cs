using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.AuditEvents;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation;

namespace xggameplan.KPIProcessing
{
    public class ScenarioCampaignMetricsProcessor : IScenarioCampaignMetricsProcessor
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISpotRepository _spotRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;

        public ScenarioCampaignMetricsProcessor(ICampaignRepository campaignRepository,
            ISpotRepository spotRepository,
            IRepositoryFactory repositoryFactory,
            IAuditEventRepository auditEventRepository)
        {
            _campaignRepository = campaignRepository;
            _spotRepository = spotRepository;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
        }

        public void ProcessScenarioCampaignMetrics(Guid runId, Guid scenarioId, IEnumerable<Recommendation> scenarioRecommendations)
        {
            try
            {
                var scenarioCampaignMetrics = new ScenarioCampaignMetric
                {
                    Id = scenarioId,
                    Metrics = GenerateCampaignKPIs(runId, scenarioId, scenarioRecommendations)
                };

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Start insert of scenario campaign metrics. ScenarioId: {scenarioId} Count: {scenarioCampaignMetrics.Metrics.Count}"));

                using (var innerScope = _repositoryFactory.BeginRepositoryScope())
                {
                    var scenarioCampaignMetricRepository = innerScope.CreateRepository<IScenarioCampaignMetricRepository>();
                    scenarioCampaignMetricRepository.AddOrUpdate(scenarioCampaignMetrics);
                    scenarioCampaignMetricRepository.SaveChanges();
                }

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"End insert of scenario campaign metrics. ScenarioId: {scenarioId} Count: {scenarioCampaignMetrics.Metrics.Count}"));
            }
            catch (Exception e)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error while processing scenario campaign metrics. ScenarioId: {scenarioId}", e));
            }
        }

        private List<ScenarioCampaignMetricItem> GenerateCampaignKPIs(Guid runId, Guid scenarioId, IEnumerable<Recommendation> scenarioRecommendations)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("Executing Campaign KPI processing (RunID={0}, ScenarioID={1})", runId, scenarioId)));

            var kpis = new List<ScenarioCampaignMetricItem>();
            var calculator = new ScenarioCampaignKPIsCalculator();

            var recommendations = scenarioRecommendations
                .GroupBy(x => x.ExternalCampaignNumber)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var campaign in _campaignRepository.GetAllFlat())
            {
                var nominalPrice = _spotRepository.GetNominalPriceByCampaign(campaign.ExternalId);
                var campaignRecommendations = recommendations.ContainsKey(campaign.ExternalId)
                    ? recommendations[campaign.ExternalId]
                    : Enumerable.Empty<Recommendation>();

                kpis.Add(calculator.CalculateCampaignKPIs(campaign, campaignRecommendations, nominalPrice));
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("Executed Campaign KPI processing (RunID={0}, ScenarioID={1})", runId, scenarioId)));
            return kpis;
        }
    }
}
