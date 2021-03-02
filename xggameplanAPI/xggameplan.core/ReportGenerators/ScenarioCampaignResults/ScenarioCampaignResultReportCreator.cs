using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignResults
{
    /// <summary>
    /// Scenario campaign result report creator.
    /// </summary>
    /// <seealso cref="xggameplan.core.ReportGenerators.ScenarioCampaignResults.ScenarioCampaignResultReportCreatorBase" />
    public class ScenarioCampaignResultReportCreator : ScenarioCampaignResultReportCreatorBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private IDictionary<string, Campaign> _campaigns;
        private IDictionary<string, Product> _products;
        private IDictionary<string, Demographic> _demographics;
        private IDictionary<string, Clash> _clashes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioCampaignResultReportCreator" /> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="clock">The clock.</param>
        public ScenarioCampaignResultReportCreator(IRepositoryFactory repositoryFactory) => _repositoryFactory = repositoryFactory;

        /// <summary>Prepares the related data.</summary>
        /// <param name="source">The source.</param>
        protected override void PrepareRelatedData(IReadOnlyCollection<ScenarioCampaignExtendedResultItem> source)
        {
            using (var innerScope = _repositoryFactory.BeginRepositoryScope())
            {
                var campaignsRepository = innerScope.CreateRepository<ICampaignRepository>();
                var demographicRepository = innerScope.CreateRepository<IDemographicRepository>();
                var productRepository = innerScope.CreateRepository<IProductRepository>();
                var clashRepository = innerScope.CreateRepository<IClashRepository>();

                _campaigns = campaignsRepository.FindByRefs(source.Select(x => x.CampaignExternalId).Distinct().ToList())
                    .ToDictionary(c => c.ExternalId);

                var productRefs = new HashSet<string>();
                var demographicRefs = new HashSet<string>();

                foreach (var campaign in _campaigns.Values)
                {
                    _ = productRefs.Add(campaign.Product);
                    _ = demographicRefs.Add(campaign.DemoGraphic);
                }

                _products = productRepository.FindByExternal(productRefs.ToList())
                    .ToDictionary(d => d.Externalidentifier);
                _demographics = demographicRepository.GetByExternalRef(demographicRefs.ToList())
                    .ToDictionary(c => c.ExternalRef);
                _clashes = clashRepository.GetAll().ToDictionary(d => d.Externalref);
            }
        }

        /// <summary>
        /// Clears cache of the related data.
        /// </summary>
        protected override void ClearRelatedData()
        {
            _campaigns = null;
            _products = null;
            _demographics = null;
            _clashes = null;
        }

        /// <summary>Resolves the campaign.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Campaign ResolveCampaign(ScenarioCampaignResultItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.CampaignExternalId))
            {
                _ = _campaigns.TryGetValue(item.CampaignExternalId, out var campaign);
                return campaign;
            }

            return null;
        }

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Product ResolveProduct(Campaign campaign)
        {
            if (!string.IsNullOrWhiteSpace(campaign.Product))
            {
                _ = _products.TryGetValue(campaign.Product, out var product);
                return product;
            }

            return null;
        }

        /// <summary>Resolves the demographic.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Demographic ResolveDemographic(Campaign campaign)
        {
            if (!string.IsNullOrWhiteSpace(campaign.DemoGraphic))
            {
                _ = _demographics.TryGetValue(campaign.DemoGraphic, out var demographic);
                return demographic;
            }

            return null;
        }

        /// <summary>Resolves the product clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Clash ResolveClash(Campaign campaign)
        {
            var product = ResolveProduct(campaign);
            if (!string.IsNullOrWhiteSpace(product?.ClashCode))
            {
                _ = _clashes.TryGetValue(product.ClashCode, out var clash);
                return clash;
            }

            return null;
        }

        /// <summary>Resolves the product parent clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Clash ResolveParentClash(Campaign campaign)
        {
            var clash = ResolveClash(campaign);
            if (!string.IsNullOrWhiteSpace(clash?.ParentExternalidentifier))
            {
                _ = _clashes.TryGetValue(clash.ParentExternalidentifier, out var parentClash);
                return parentClash;
            }

            return null;
        }

        /// <summary>
        /// Resolves PayPart for pre-post KPI calculation
        /// </summary>
        /// <param name="campaign"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks>Example of DowTime string inside DayPartName: "2100-2659(Mon-Fri)"</remarks>
        protected override ICampaignKpiData ResolveDayPartKpiModel(Campaign campaign, ScenarioCampaignResultItem item)
        {
            var salesAreaCampaignTarget = campaign.SalesAreaCampaignTarget.FirstOrDefault(s => s.SalesArea == item.SalesAreaName);

            if (salesAreaCampaignTarget is null)
            {
                return null;
            }

            var campaignStrikeWeights = KPICalculationHelpers.GetSalesAreaCampaignTargetStrikeWeights(salesAreaCampaignTarget);

            var campaignStrikeWeight = campaignStrikeWeights.FirstOrDefault(s => s.StartDate == item.StrikeWeightStartDate
                && s.EndDate == item.StrikeWeightEndDate);

            if (campaignStrikeWeight is null)
            {
                return null;
            }

            return KPICalculationHelpers.GetDayPartByDowTimeString(campaignStrikeWeight.DayParts, item.DaypartName);
        }
    }
}
