using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using CampaignStatus = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.CampaignStatus;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Campaign repository which takes into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    public class LandmarkCampaignRepository : CampaignRepository
    {
        private readonly ISqlServerLongRunningTenantDbContext _dbContext;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;

        /// <summary>Initializes a new instance of the <see cref="LandmarkCampaignRepository" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="searchConditionBuilder">The search condition builder.</param>
        /// <param name="identityGenerator">The identity generator.</param>
        /// <param name="mapper">The mapper.</param>
        public LandmarkCampaignRepository(
            ISqlServerLongRunningTenantDbContext dbContext,
            IFullTextSearchConditionBuilder searchConditionBuilder,
            IIdentityGenerator identityGenerator,
            IMapper mapper)
            : base(dbContext, searchConditionBuilder, identityGenerator, mapper)
        {
            _dbContext = dbContext;
            _searchConditionBuilder = searchConditionBuilder;
        }

        /// <summary>
        /// Prepares <see cref="CampaignSearchDto"/> query takin into account
        /// Landmark Product related data by its active periods.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns></returns>
        protected override IQueryable<CampaignSearchDto> GetSearchDtoQuery(CampaignSearchQueryModel queryModel)
        {
            var query =
                from campaign in _dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                join campaignWithProductRelations in _dbContext.Specific.View<CampaignWithProductRelations>() on
                    campaign.Id equals campaignWithProductRelations.CampaignId
                join productJoin in _dbContext.Query<Product>() on campaignWithProductRelations.ProductId equals
                    productJoin.Id into products
                from product in products.DefaultIfEmpty()
                join demographicJoin in _dbContext.Query<Demographic>() on campaign.Demographic equals demographicJoin
                    .ExternalRef into demographics
                from demographic in demographics.DefaultIfEmpty()
                join clashCodeJoin in _dbContext.Query<Clash>() on product.ClashCode equals clashCodeJoin.Externalref
                    into clashes
                from clash in clashes.DefaultIfEmpty()
                join advertiserJoin in _dbContext.Query<Advertiser>() on campaignWithProductRelations.AdvertiserId
                    equals advertiserJoin.Id into advertisers
                from advertiser in advertisers.DefaultIfEmpty()
                join agencyJoin in _dbContext.Query<Agency>() on campaignWithProductRelations.AgencyId equals agencyJoin
                    .Id into agencies
                from agency in agencies.DefaultIfEmpty()
                join agencyGroupJoin in _dbContext.Query<AgencyGroup>() on campaignWithProductRelations.AgencyGroupId
                    equals agencyGroupJoin.Id into agencyGroups
                from agencyGroup in agencyGroups.DefaultIfEmpty()
                join personJoin in _dbContext.Query<Person>() on campaignWithProductRelations.PersonId equals personJoin
                    .Id into persons
                from person in persons.DefaultIfEmpty()

                select new CampaignEntitySearchQueryModel
                {
                    Campaign = campaign,
                    Product = product,
                    Demographic = demographic,
                    Clash = clash,
                    Advertiser = advertiser,
                    Agency = agency,
                    AgencyGroup = agencyGroup,
                    Person = person
                };

            ApplyCampaignStatusFilter();
            ApplyCampaignPeriodFilter();
            ApplyCampaignSearchFilter();

            return query.Select(q => new CampaignSearchDto
            {
                Uid = q.Campaign.Id,
                CustomId = q.Campaign.CustomId,
                Status = q.Campaign.Status,
                Name = q.Campaign.Name,
                ExternalId = q.Campaign.ExternalId,
                CampaignGroup = q.Campaign.CampaignGroup,
                StartDateTime = q.Campaign.StartDateTime,
                EndDateTime = q.Campaign.EndDateTime,
                ProductExternalRef = q.Product.Externalidentifier,
                ProductName = q.Product.Name,
                AdvertiserName = q.Advertiser.Name,
                AgencyName = q.Agency.Name,
                ReportingCategory = q.Product.ReportingCategory,
                MediaGroupCode = q.AgencyGroup.Code,
                MediaGroupShortName = q.AgencyGroup.ShortName,
                ProductAssigneeName = q.Person.Name,
                BusinessType = q.Campaign.BusinessType,
                DeliveryType = q.Campaign.DeliveryType,
                Demographic = q.Demographic.ExternalRef,
                RevenueBudget = q.Campaign.RevenueBudget,
                TargetRatings = q.Campaign.TargetRatings,
                ActualRatings = q.Campaign.ActualRatings,
                IsPercentage = q.Campaign.IsPercentage,
                IncludeOptimisation = q.Campaign.IncludeOptimisation,
                TargetZeroRatedBreaks = q.Campaign.TargetZeroRatedBreaks,
                InefficientSpotRemoval = q.Campaign.InefficientSpotRemoval,
                IncludeRightSizer = q.Campaign.IncludeRightSizer,
                RightSizerLevel = q.Campaign.RightSizerLevel,
                CampaignPassPriority = q.Campaign.CampaignPassPriority,
                ClashCode = q.Clash.Externalref,
                ClashDescription = q.Clash.Description,
                StopBooking = q.Campaign.StopBooking,
                TargetXP = q.Campaign.TargetXP,
                RevenueBooked = q.Campaign.RevenueBooked,
                CreationDate = q.Campaign.CreationDate,
                AutomatedBooked = q.Campaign.AutomatedBooked,
                TopTail = q.Campaign.TopTail,
                Spots = q.Campaign.Spots,
                CampaignPaybacks = q.Campaign.CampaignPaybacks,
                ActiveLength = q.Campaign.ActiveLength,
                RatingsDifferenceExcludingPayback = q.Campaign.RatingsDifferenceExcludingPayback,
                ValueDifference = q.Campaign.ValueDifference,
                ValueDifferenceExcludingPayback = q.Campaign.ValueDifferenceExcludingPayback,
                AchievedPercentageTargetRatings = q.Campaign.AchievedPercentageTargetRatings,
                AchievedPercentageRevenueBudget = q.Campaign.AchievedPercentageRevenueBudget
            });

            void ApplyCampaignStatusFilter()
            {
                if (queryModel is null)
                {
                    return;
                }

                // add query condition for the specified campaign status
                if (queryModel.Status != Domain.Campaigns.CampaignStatus.All)
                {
                    query = query.Where(q => q.Campaign.Status == (CampaignStatus)(int)queryModel.Status);
                }
            }

            void ApplyCampaignPeriodFilter()
            {
                if (queryModel is null)
                {
                    return;
                }

                // add query condition if start and end dates are defined and start less than end
                if (queryModel.StartDate != default && queryModel.EndDate != default &&
                    queryModel.StartDate <= queryModel.EndDate)
                {
                    query = query.Where(q =>
                        q.Campaign.StartDateTime <= queryModel.EndDate.AddDays(1) &&
                        q.Campaign.EndDateTime >= queryModel.StartDate.Date);
                }
            }

            void ApplyCampaignSearchFilter()
            {
                if (queryModel is null)
                {
                    return;
                }

                query = ApplyCampaignSearchFilters(queryModel, query);
            }
        }
    }
}
