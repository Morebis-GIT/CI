using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.core.ReportGenerators.ScenarioCampaignResults;
using AdvertiserEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Advertiser;
using AgencyEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Agency;
using Campaign = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.Campaign;
using CampaignEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.Campaign;
using ClashEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Clash;
using DemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Demographic;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Scenario campaign result report creator takes into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    /// <seealso cref="xggameplan.core.ReportGenerators.ScenarioCampaignResults.ScenarioCampaignResultReportCreatorBase" />
    public class ScenarioCampaignResultReportCreator : ScenarioCampaignResultReportCreatorBase
    {
        private static readonly Dictionary<DayOfWeek, string> _dayStrings = new Dictionary<DayOfWeek, string>
        {
            {DayOfWeek.Monday, "Mon"},
            {DayOfWeek.Tuesday, "Tue"},
            {DayOfWeek.Wednesday, "Wed"},
            {DayOfWeek.Thursday, "Thu"},
            {DayOfWeek.Friday, "Fri"},
            {DayOfWeek.Saturday, "Sat"},
            {DayOfWeek.Sunday, "Sun"}
        };

        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly ISqlServerSalesAreaByNullableIdCacheAccessor _salesAreaByNullableIdCache;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly IMapper _mapper;

        private IDictionary<string, ProductLinkDomain> _productLinks;
        private IDictionary<string, List<CampaignDayPartKpi>> _dayPartKpiLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioCampaignResultReportCreator" /> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        /// <param name="mapper">The mapper.</param>
        public ScenarioCampaignResultReportCreator(
            ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            ISqlServerSalesAreaByNullableIdCacheAccessor salesAreaByNullableIdCache,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            IMapper mapper)
        {
            _dbContextFactory = dbContextFactory;
            _salesAreaByNullableIdCache = salesAreaByNullableIdCache;
            _salesAreaByIdCache = salesAreaByIdCache;
            _mapper = mapper;
        }

        /// <summary>Prepares the related data.</summary>
        /// <param name="source">The source.</param>
        protected override void PrepareRelatedData(IReadOnlyCollection<ScenarioCampaignExtendedResultItem> source)
        {
            var campaignExternalIds = source
                .Select(x => x.CampaignExternalId)
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            using var dbContext = _dbContextFactory.Create();
            _productLinks =
                (from campaignWithProductRelations in dbContext.Specific.View<CampaignWithProductRelations>()
                 join campaign in dbContext.Query<CampaignEntity>()
                         .Include(x => x.SalesAreaCampaignTargets)
                         .ThenInclude(x => x.SalesAreaGroup)
                         .ThenInclude(x => x.SalesAreas)
                         .Include(x => x.CampaignPaybacks)
                     on campaignWithProductRelations.CampaignId equals campaign.Id
                 join demographicJoin in dbContext.Query<DemographicEntity>() on campaign.Demographic equals
                     demographicJoin.ExternalRef into dJoin
                 from demographic in dJoin.DefaultIfEmpty()
                 join productJoin in dbContext.Query<ProductEntity>() on campaignWithProductRelations.ProductId equals
                     productJoin.Uid into pJoin
                 from product in pJoin.DefaultIfEmpty()
                 join clashJoin in dbContext.Query<ClashEntity>() on product.ClashCode equals
                     clashJoin.Externalref into clJoin
                 from clash in clJoin.DefaultIfEmpty()
                 join parentClashJoin in dbContext.Query<ClashEntity>() on clash.ParentExternalidentifier equals
                     parentClashJoin.Externalref into pclJoin
                 from parentClash in pclJoin.DefaultIfEmpty()
                 join advertiserJoin in dbContext.Query<AdvertiserEntity>() on campaignWithProductRelations.AdvertiserId equals
                     advertiserJoin.Id into adJoin
                 from advertiser in adJoin.DefaultIfEmpty()
                 join agencyJoin in dbContext.Query<AgencyEntity>() on campaignWithProductRelations.AgencyId equals
                     agencyJoin.Id into agJoin
                 from agency in agJoin.DefaultIfEmpty()
                 where campaignExternalIds.Contains(campaign.ExternalId)
                 select new ProductLinkSql
                 {
                     Campaign = campaign,
                     Demographic = demographic,
                     Clash = clash,
                     ParentClash = parentClash,
                     Product = product,
                     Advertiser = advertiser,
                     Agency = agency
                 })
            .AsNoTracking()
            .ToDictionary(x => x.Campaign.ExternalId, x => new ProductLinkDomain
            {
                Campaign = _mapper.Map<Campaign>(x.Campaign, opts => opts.UseEntityCache(_salesAreaByIdCache)),
                Demographic = x.Demographic is null
                    ? null
                    : _mapper.Map<Demographic>(x.Demographic),
                Product = x.Product is null
                    ? null
                    : new Product
                    {
                        Name = x.Product.Name,
                        AdvertiserName = x.Advertiser?.Name,
                        AgencyName = x.Agency?.Name
                    },
                Clash = x.Clash is null
                    ? null
                    : _mapper.Map<Clash>(x.Clash, opts => opts.UseEntityCache(_salesAreaByNullableIdCache)),
                ParentClash = x.ParentClash is null
                    ? null
                    : _mapper.Map<Clash>(x.ParentClash, opts => opts.UseEntityCache(_salesAreaByNullableIdCache))
            });

            _dayPartKpiLinks = (from campaign in dbContext.Query<CampaignEntity>()
                                join salesAreaTargetJoin in dbContext.Query<CampaignSalesAreaTarget>() on campaign.Id equals
                                    salesAreaTargetJoin.CampaignId into sJoin
                                from campaignSalesArea in sJoin.DefaultIfEmpty()
                                join campaignTargetJoin in dbContext.Query<CampaignTarget>() on campaignSalesArea.Id equals
                                    campaignTargetJoin.CampaignSalesAreaTargetId into tJoin
                                from campaignTarget in tJoin.DefaultIfEmpty()
                                join strikeWeightJoin in dbContext.Query<CampaignTargetStrikeWeight>() on campaignTarget.Id equals
                                    strikeWeightJoin.CampaignTargetId into swJoin
                                from strikeWeight in swJoin.DefaultIfEmpty()
                                join dayPartJoin in dbContext.Query<CampaignTargetStrikeWeightDayPart>() on strikeWeight.Id equals
                                    dayPartJoin.CampaignTargetStrikeWeightId into dpJoin
                                from dayPart in dpJoin.DefaultIfEmpty()
                                join timeSliceJoin in dbContext.Query<CampaignTargetStrikeWeightDayPartTimeSlice>() on dayPart.Id equals
                                    timeSliceJoin.CampaignTargetStrikeWeightDayPartId into tsJoin
                                from timeSlice in tsJoin.DefaultIfEmpty()
                                where campaignExternalIds.Contains(campaign.ExternalId)
                                select new CampaignDayPartKpi
                                {
                                    CampaignExternalId = campaign.ExternalId,
                                    CampaignSalesArea = campaignSalesArea.SalesArea.Name,
                                    StrikeWeightStartDate = strikeWeight.StartDate,
                                    StrikeWeightEndDate = strikeWeight.EndDate,
                                    NominalValue = dayPart.NominalValue,
                                    Payback = dayPart.Payback,
                                    TimeSliceFromTime = _mapper.Map<string>(timeSlice.FromTime),
                                    TimeSliceToTime = _mapper.Map<string>(timeSlice.ToTime),
                                    DowPattern = new SortedSet<string>(timeSlice.DowPattern.Select(x => _dayStrings[x]))
                                })
                .AsNoTracking()
                .ToArray()
                .GroupBy(x => x.DayPartNameHashSet)
                .ToDictionary(x => x.Key, x => x.ToList());
        }

        /// <summary>
        /// Clears cache of the related data.
        /// </summary>
        protected override void ClearRelatedData()
        {
            _productLinks = null;
            _dayPartKpiLinks = null;
        }

        /// <summary>Resolves the campaign.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override Campaign ResolveCampaign(ScenarioCampaignResultItem item) =>
            !string.IsNullOrWhiteSpace(item.CampaignExternalId) &&
            _productLinks.TryGetValue(item.CampaignExternalId, out var productLink)
                ? productLink.Campaign
                : null;

        /// <summary>Resolves the product.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Product ResolveProduct(Campaign campaign) =>
            _productLinks.TryGetValue(campaign.ExternalId, out var productLink)
                ? productLink.Product
                : null;

        /// <summary>Resolves the demographic.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Demographic ResolveDemographic(Campaign campaign) =>
            _productLinks.TryGetValue(campaign.ExternalId, out var productLink)
                ? productLink.Demographic
                : null;

        /// <summary>Resolves the product clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Clash ResolveClash(Campaign campaign) =>
            _productLinks.TryGetValue(campaign.ExternalId, out var productLink)
                ? productLink.Clash
                : null;

        /// <summary>Resolves the product parent clash.</summary>
        /// <param name="campaign">The campaign.</param>
        /// <returns></returns>
        protected override Clash ResolveParentClash(Campaign campaign) =>
            _productLinks.TryGetValue(campaign.ExternalId, out var productLink)
                ? productLink.ParentClash
                : null;

        /// <summary>
        /// Resolves PayPart for pre-post KPI calculation
        /// </summary>
        /// <param name="campaign"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks>Example of DowTime string inside DayPartName: "2100-2659(Mon-Fri)"</remarks>
        protected override ICampaignKpiData ResolveDayPartKpiModel(Campaign campaign, ScenarioCampaignResultItem item)
        {
            if (item.DaypartName is null || item.DaypartName == "NotSupplied" || item.DaypartName.Length < 18)
            {
                return null;
            }

            var agFromTime = item.DaypartName.Substring(0, 4);
            var agToTime = item.DaypartName.Substring(5, 4);
            var fromTime = AgConversions.ParseTotalHHMMSSFormat(agFromTime + "00", false).ToString(@"hh\:mm\:ss");
            var toTime = AgConversions.ParseTotalHHMMSSFormat(agToTime + "59", false).ToString(@"hh\:mm\:ss");
            var firstDow = item.DaypartName.Substring(10, 3);
            var lastDow = item.DaypartName.Substring(14, 3);

            var dayPartLinkKey = $"{campaign.ExternalId}{item.SalesAreaName}{item.StrikeWeightStartDate}{item.StrikeWeightEndDate}{fromTime}{toTime}";

            return _dayPartKpiLinks.TryGetValue(dayPartLinkKey, out var dayPartKpiList)
                ? dayPartKpiList.FirstOrDefault(x => x.DowPattern.Contains(firstDow) && x.DowPattern.Contains(lastDow))
                : null;
        }

        private class ProductLinkSql
        {
            public CampaignEntity Campaign { get; set; }
            public DemographicEntity Demographic { get; set; }
            public ProductEntity Product { get; set; }
            public ClashEntity Clash { get; set; }
            public ClashEntity ParentClash { get; set; }
            public AdvertiserEntity Advertiser { get; set; }
            public AgencyEntity Agency { get; set; }
        }

        private class ProductLinkDomain
        {
            public Campaign Campaign { get; set; }
            public Demographic Demographic { get; set; }
            public Product Product { get; set; }
            public Clash Clash { get; set; }
            public Clash ParentClash { get; set; }
        }

        private class CampaignDayPartKpi : ICampaignKpiData
        {
            public string CampaignExternalId { get; set; }
            public string CampaignSalesArea { get; set; }
            public DateTime StrikeWeightStartDate { get; set; }
            public DateTime StrikeWeightEndDate { get; set; }
            public double NominalValue { get; set; }
            public double? Payback { get; set; }
            public double? RevenueBudget { get; set; }
            public string TimeSliceFromTime { get; set; }
            public string TimeSliceToTime { get; set; }
            public SortedSet<string> DowPattern { get; set; } = new SortedSet<string>();
            public string DayPartNameHashSet => $"{CampaignExternalId}{CampaignSalesArea}{StrikeWeightStartDate}{StrikeWeightEndDate}{TimeSliceFromTime}{TimeSliceToTime}";
        }
    }
}
