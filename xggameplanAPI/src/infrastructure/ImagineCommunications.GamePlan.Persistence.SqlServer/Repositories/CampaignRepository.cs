using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using xggameplan.core.Extensions.AutoMapper;
using Campaign = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.Campaign;
using CampaignStatus = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.CampaignStatus;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private const int MaxClauseCount = 1000;
        protected readonly ISqlServerLongRunningTenantDbContext _dbContext;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly IIdentityGenerator _identityGenerator;
        private readonly SequenceRebuilder<Entities.Tenant.Campaigns.Campaign, CampaignNoIdentity> _sequenceRebuilder;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        protected readonly IMapper _mapper;

        private IQueryable<Entities.Tenant.Campaigns.Campaign> CampaignQuery =>
            _dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                .Include(o => o.BreakRequirement).ThenInclude(o => o.CentreBreakRequirement)
                .Include(o => o.BreakRequirement).ThenInclude(o => o.EndBreakRequirement)
                .Include(o => o.SalesAreaCampaignTargets).ThenInclude(o => o.Multiparts).ThenInclude(o => o.Lengths)
                .Include(o => o.SalesAreaCampaignTargets).ThenInclude(o => o.CampaignTargets).ThenInclude(o => o.StrikeWeights).ThenInclude(o => o.Lengths)
                .Include(o => o.SalesAreaCampaignTargets).ThenInclude(o => o.CampaignTargets).ThenInclude(o => o.StrikeWeights).ThenInclude(o => o.DayParts).ThenInclude(o => o.Lengths)
                .Include(o => o.SalesAreaCampaignTargets).ThenInclude(o => o.CampaignTargets).ThenInclude(o => o.StrikeWeights).ThenInclude(o => o.DayParts).ThenInclude(o => o.Timeslices)
                .Include(o => o.SalesAreaCampaignTargets).ThenInclude(o => o.SalesAreaGroup).ThenInclude(o => o.SalesAreas)
                .Include(o => o.BreakTypes)
                .Include(o => o.ProgrammeRestrictions).ThenInclude(o => o.CategoryOrProgramme)
                .Include(o => o.ProgrammeRestrictions).ThenInclude(o => o.SalesAreas)
                .Include(o => o.TimeRestrictions).ThenInclude(o => o.SalesAreas)
                .Include(o => o.BookingPositionGroups).ThenInclude(o => o.SalesAreas)
                .Include(o => o.CampaignPaybacks);

        public CampaignRepository(ISqlServerLongRunningTenantDbContext dbContext,
            IFullTextSearchConditionBuilder searchConditionBuilder,
            IIdentityGenerator identityGenerator,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _searchConditionBuilder = searchConditionBuilder;
            _identityGenerator = identityGenerator;
            _sequenceRebuilder = new SequenceRebuilder<Entities.Tenant.Campaigns.Campaign, CampaignNoIdentity>();
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public Campaign Add(Campaign item)
        {
            PrepareCampaignsToSave(new List<Campaign> { item });

            _ = _dbContext.Add(_mapper.Map<Entities.Tenant.Campaigns.Campaign>(item, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

            return item;
        }

        public virtual void Add(IEnumerable<Campaign> item)
        {
            PrepareCampaignsToSave(item);

            _dbContext.AddRange(_mapper.Map<Entities.Tenant.Campaigns.Campaign[]>(item, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
        }

        protected void PrepareCampaignsToSave(IEnumerable<Campaign> items)
        {
            var entitiesWithoutCustomId = items.Where(e => e.CustomId == 0);

            var newIdentities = new Stack<int>(
                _identityGenerator.GetIdentities<CampaignNoIdentity>(
                    entitiesWithoutCustomId.Count()
                ).Select(e => e.Id)
            );

            foreach (var campaign in entitiesWithoutCustomId)
            {
                campaign.CustomId = newIdentities.Pop();
            }

            foreach (var campaign in items)
            {
                campaign.UpdateDerivedKPIs();
            }
        }

        public PagedQueryResult<CampaignWithProductFlatModel> GetWithProduct(CampaignSearchQueryModel queryModel)
        {
            var res = _mapper.Map<List<CampaignWithProductFlatModel>>(GetSearchDtoQuery(queryModel).AsNoTracking(), opts => opts.UseEntityCache(_salesAreaByIdCache));

            if (queryModel != null && queryModel.GroupByGroupName)
            {
                res = GroupByCampaignGroup(res);
            }

            return new PagedQueryResult<CampaignWithProductFlatModel>(res.Count,
                res.ApplySortingAndPaging(queryModel).ToList());
        }

        private List<CampaignWithProductFlatModel> GroupByCampaignGroup(IList<CampaignWithProductFlatModel> items)
        {
            if (items == null)
            {
                return new List<CampaignWithProductFlatModel>();
            }
            var result = (from item in items.Where(x => !String.IsNullOrEmpty(x.CampaignGroup))
                          group item by new
                          {
                              item.CampaignGroup,
                              item.Status,
                              item.StartDateTime,
                              item.EndDateTime,
                              item.ProductExternalRef,
                              item.ProductName,
                              item.AdvertiserName,
                              item.AgencyName,
                              item.ReportingCategory,
                              item.ProductAssigneeName,
                              item.MediaGroup,
                              item.BusinessType,
                              item.Demographic,
                              item.RevenueBudget,
                              item.IsPercentage,
                              item.IncludeOptimisation,
                              item.InefficientSpotRemoval,
                              item.IncludeRightSizer,
                              item.ClashCode,
                              item.ClashDescription
                          } into groupedByItems
                          select new CampaignWithProductFlatModel
                          {
                              CampaignGroup = groupedByItems.Key.CampaignGroup,
                              Uid = groupedByItems.First().Uid,
                              CustomId = groupedByItems.First().CustomId,
                              ExternalId = groupedByItems.Select(x => x.ExternalId).Distinct().Count() == 1 ? groupedByItems.Select(x => x.ExternalId).First() : string.Empty,
                              Status = groupedByItems.Key.Status,
                              Name = groupedByItems.Select(x => x.Name).Distinct().Count() == 1 ? groupedByItems.Select(x => x.Name).First() : string.Empty,
                              StartDateTime = groupedByItems.Key.StartDateTime,
                              EndDateTime = groupedByItems.Key.EndDateTime,
                              ProductExternalRef = groupedByItems.Key.ProductExternalRef,
                              ProductName = groupedByItems.Key.ProductName,
                              AdvertiserName = groupedByItems.Key.AdvertiserName,
                              AgencyName = groupedByItems.Key.AgencyName,
                              ReportingCategory = groupedByItems.Key.ReportingCategory,
                              ProductAssigneeName = groupedByItems.Key.ProductAssigneeName,
                              MediaGroup = groupedByItems.Key.MediaGroup,
                              BusinessType = groupedByItems.Key.BusinessType,
                              Demographic = groupedByItems.Key.Demographic,
                              RevenueBudget = groupedByItems.Key.RevenueBudget,
                              IsPercentage = groupedByItems.Key.IsPercentage,
                              IncludeOptimisation = groupedByItems.Key.IncludeOptimisation,
                              InefficientSpotRemoval = groupedByItems.Key.InefficientSpotRemoval,
                              IncludeRightSizer = groupedByItems.Key.IncludeRightSizer,
                              DefaultCampaignPassPriority = groupedByItems.First().DefaultCampaignPassPriority,
                              ClashCode = groupedByItems.Key.ClashCode,
                              ClashDescription = groupedByItems.Key.ClashDescription,
                              TargetRatings = groupedByItems.Sum(x => x.TargetRatings),
                              ActualRatings = groupedByItems.Sum(x => x.ActualRatings),
                              ActiveLength = groupedByItems.AggregateActiveLength(),
                              RatingsDifferenceExcludingPayback = groupedByItems.Sum(e => e.RatingsDifferenceExcludingPayback),
                              ValueDifference = groupedByItems.Sum(e => e.ValueDifference),
                              ValueDifferenceExcludingPayback = groupedByItems.Sum(e => e.ValueDifferenceExcludingPayback),
                              AchievedPercentageTargetRatings = groupedByItems.AggregateAchievedPercentageTargetRatings(),
                              AchievedPercentageRevenueBudget = groupedByItems.AggregateAchievedPercentageRevenueBudget()
                          }).ToList();
            if (items.Any(x => String.IsNullOrEmpty(x.CampaignGroup)))
            {
                result.AddRange(items.Where(x => String.IsNullOrEmpty(x.CampaignGroup)));
            }
            return result;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.Campaigns.Campaign>().Count();

        public int CountAllActive => _dbContext.Query<Entities.Tenant.Campaigns.Campaign>().Count(CampaignProfile.IsActivePredicate);

        public IEnumerable<Campaign> GetAll() =>
            FindByRefs(_dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                .Select(x => x.ExternalId)
                .AsNoTracking().ToList());

        public IEnumerable<CampaignReducedModel> GetAllFlat() =>
            _mapper.Map<List<CampaignReducedModel>>(
                _dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                    .Include(o => o.SalesAreaCampaignTargets).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Campaign> GetAllActive() =>
            FindByRefs(_dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                .Where(CampaignProfile.IsActivePredicate)
                .Select(x => x.ExternalId)
                .AsNoTracking().ToList());

        public IEnumerable<Campaign> GetAllScenarioUI() =>
            _mapper.Map<List<Campaign>>(
                CampaignQuery
                .Where(c => c.Status != CampaignStatus.Cancelled && c.ActualRatings >= c.TargetRatings &&
                            c.SalesAreaCampaignTargets.Any()).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<string> GetAllActiveExternalIds() =>
            _dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                .Where(c => c.ExternalId != null && c.Status == CampaignStatus.Active).Select(c => c.ExternalId)
                .ToList();

        public Campaign Find(Guid uid) => Get(uid);

        public Campaign Get(Guid uid) => _mapper.Map<Campaign>(
                CampaignQuery
                .FirstOrDefault(c => c.Id == uid),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Campaign> Find(List<Guid> uids) =>
            _mapper.Map<List<Campaign>>(
                CampaignQuery
                .Where(c => uids.Contains(c.Id)).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Campaign> FindByRef(string externalRef) =>
            _mapper.Map<List<Campaign>>(
                CampaignQuery
                .Where(c => c.ExternalId == externalRef),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Campaign> GetByGroup(string group) =>
            _mapper.Map<List<Campaign>>(
                CampaignQuery
                .Where(c => c.CampaignGroup == group).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Campaign> FindByRefs(List<string> externalRefs) =>
            GroupElementsForClause(externalRefs).SelectMany(group =>
            {
                var ids = group.ToList();

                return _mapper.Map<List<Campaign>>(
                    CampaignQuery
                    .Where(c => ids.Contains(c.ExternalId)).AsNoTracking(),
                    opts => opts.UseEntityCache(_salesAreaByIdCache));
            }).ToList();

        public IEnumerable<Campaign> FindMissingCampaignsFromGroup(List<string> externalRefs, List<string> campaignGroup)
        {
            return GroupElementsForClause(campaignGroup).SelectMany(group =>
            _mapper.Map<List<Campaign>>(
                CampaignQuery
                .Where(c => group.Contains(c.CampaignGroup)).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache))
                .Where(r => !externalRefs.Contains(r.ExternalId))
            ).ToList();
        }

        public IEnumerable<CampaignNameModel> FindNameByRefs(ICollection<string> externalRefs)
        {
            var distinctExternalRefs = externalRefs.Distinct().ToList();
            var campaignNames = new List<CampaignNameModel>();
            for (int i = 0, page = 0; i < distinctExternalRefs.Count; i += MaxClauseCount, page++)
            {
                var refs = distinctExternalRefs.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                campaignNames.AddRange(_dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                    .Where(x => refs.Contains(x.ExternalId)).ProjectTo<CampaignNameModel>(_mapper.ConfigurationProvider));
            }

            return campaignNames;
        }

        public IEnumerable<string> GetBusinessTypes() =>
            _dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                .Where(c => !string.IsNullOrEmpty(c.BusinessType))
                .Select(c => c.BusinessType)
                .Distinct()
                .ToList();

        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            var entity = _dbContext.Find<Entities.Tenant.Campaigns.Campaign>(uid);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void Delete(IEnumerable<string> campaignExternalIds)
        {
            var groups = GroupElementsForClause(campaignExternalIds.Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim()));
            var campaigns = groups.SelectMany(group =>
                _dbContext.Query<Entities.Tenant.Campaigns.Campaign>().Where(c => group.Contains(c.ExternalId))
                    .AsEnumerable()).ToArray();
            _dbContext.RemoveRange(campaigns);
        }

        public void Truncate()
        {
            _dbContext.Truncate<Entities.Tenant.Campaigns.Campaign>();
        }

        public Task TruncateAsync() => Task.Run(Truncate);

        public void Update(Campaign campaign)
        {
            var entity = _dbContext.Find<Entities.Tenant.Campaigns.Campaign>(campaign.Id);
            if (entity != null)
            {
                campaign.UpdateDerivedKPIs();
                _ = _mapper.Map(campaign, entity, opts => opts.IgnoreCollections().UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity);
            }
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
            _sequenceRebuilder.Execute(_dbContext, e => e.CustomId);
        }

        protected virtual IQueryable<CampaignSearchDto> GetSearchDtoQuery(CampaignSearchQueryModel queryModel)
        {
            var query =
                from campaign in _dbContext.Query<Entities.Tenant.Campaigns.Campaign>()
                join productJoin in _dbContext.Query<Product>() on campaign.Product equals productJoin
                    .Externalidentifier into products
                from product in products.DefaultIfEmpty()
                join demographicJoin in _dbContext.Query<Demographic>() on campaign.Demographic equals demographicJoin
                    .ExternalRef into demographics
                from demographic in demographics.DefaultIfEmpty()
                join clashJoin in _dbContext.Query<Clash>() on product.ClashCode equals clashJoin.Externalref into
                    clashes
                from clash in clashes.DefaultIfEmpty()
                join productAdvertiserJoin in _dbContext.Query<ProductAdvertiser>() on product.Uid equals
                    productAdvertiserJoin.ProductId into paJoin
                from productAdvertiser in paJoin.DefaultIfEmpty()
                join advertiserJoin in _dbContext.Query<Advertiser>() on productAdvertiser.AdvertiserId equals
                    advertiserJoin.Id into advertisers
                from advertiser in advertisers.DefaultIfEmpty()
                join productAgencyJoin in _dbContext.Query<ProductAgency>() on product.Uid equals productAgencyJoin
                    .ProductId into pagJoin
                from productAgency in pagJoin.DefaultIfEmpty()
                join agencyJoin in _dbContext.Query<Agency>() on productAgency.AgencyId equals agencyJoin.Id into
                    agencies
                from agency in agencies.DefaultIfEmpty()
                join agencyGroupJoin in _dbContext.Query<AgencyGroup>() on productAgency.AgencyGroupId equals
                    agencyGroupJoin.Id into agencyGroups
                from agencyGroup in agencyGroups.DefaultIfEmpty()
                join productPersonJoin in _dbContext.Query<ProductPerson>() on product.Uid equals productPersonJoin
                    .ProductId into ppJoin
                from productPerson in ppJoin.DefaultIfEmpty()
                join personJoin in _dbContext.Query<Person>() on productPerson.PersonId equals personJoin.Id into
                    persons
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

            if (queryModel != null)
            {
                if (queryModel.Status != Domain.Campaigns.CampaignStatus.All)
                {
                    query = query.Where(q => q.Campaign.Status == (CampaignStatus)(int)queryModel.Status);
                }

                if (queryModel.StartDate != default && queryModel.EndDate != default &&
                    queryModel.StartDate <= queryModel.EndDate)
                {
                    query = query.Where(q =>
                        q.Campaign.StartDateTime <= queryModel.EndDate.AddDays(1) &&
                        q.Campaign.EndDateTime >= queryModel.StartDate.Date);
                }

                query = ApplyCampaignSearchFilters(queryModel, query);
            }

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
        }

        private static IEnumerable<IGrouping<int, string>> GroupElementsForClause(IEnumerable<string> elements) =>
            elements
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / MaxClauseCount, x => x.Item);

        protected IQueryable<CampaignEntitySearchQueryModel> ApplyCampaignSearchFilters(CampaignSearchQueryModel queryModel, IQueryable<CampaignEntitySearchQueryModel> query)
        {
            if (!string.IsNullOrWhiteSpace(queryModel.Description))
            {
                var ftsQueryText = _searchConditionBuilder.StartWith(queryModel.Description).Build();

                query = query.Where(q =>
                    //campaign
                    EF.Functions.Contains(
                        EF.Property<string>(q.Campaign, Entities.Tenant.Campaigns.Campaign.SearchTokensFieldName),
                        ftsQueryText) ||
                    q.Campaign.CampaignGroup.Contains(queryModel.Description) ||
                    q.Campaign.Name.Contains(queryModel.Description) ||
                    q.Campaign.ExternalId.Contains(queryModel.Description) ||
                    q.Campaign.BusinessType.Contains(queryModel.Description) ||
                    //demographics
                    q.Demographic.ShortName.Contains(queryModel.Description) ||
                    //product
                    q.Product.Name.Contains(queryModel.Description) ||
                    EF.Functions.Contains(EF.Property<string>(q.Product, Product.SearchFieldName),
                        ftsQueryText) ||
                    //advertiser
                    q.Advertiser.Name.Contains(queryModel.Description) ||
                    EF.Functions.Contains(EF.Property<string>(q.Advertiser, Advertiser.SearchFieldName),
                        ftsQueryText) ||
                    //agency
                    q.Agency.Name.Contains(queryModel.Description) ||
                    EF.Functions.Contains(EF.Property<string>(q.Agency, Agency.SearchFieldName),
                        ftsQueryText) ||
                    //clash
                    q.Clash.Externalref.Contains(queryModel.Description) ||
                    q.Clash.Description.Contains(queryModel.Description)
                );
            }

            if (queryModel.BusinessTypes != null && queryModel.BusinessTypes.Any())
            {
                query = query.Where(q => queryModel.BusinessTypes.Contains(q.Campaign.BusinessType));
            }

            if (queryModel.ClashCodes != null && queryModel.ClashCodes.Any())
            {
                query = query.Where(q => queryModel.ClashCodes.Contains(q.Product.ClashCode));
            }

            if (queryModel.CampaignIds != null && queryModel.CampaignIds.Any())
            {
                query = query.Where(q => queryModel.CampaignIds.Contains(q.Campaign.ExternalId));
            }

            if (queryModel.ProductIds != null && queryModel.ProductIds.Any())
            {
                query = query.Where(q => queryModel.ProductIds.Contains(q.Campaign.Product));
            }

            if (queryModel.AgencyIds != null && queryModel.AgencyIds.Any())
            {
                query = query.Where(q => queryModel.AgencyIds.Contains(q.Agency.ExternalIdentifier));
            }

            if (queryModel.MediaSalesGroupIds != null && queryModel.MediaSalesGroupIds.Any())
            {
                query = query.Where(q => queryModel.MediaSalesGroupIds.Contains(q.AgencyGroup.Code));
            }

            if (queryModel.ProductAssigneeIds != null && queryModel.ProductAssigneeIds.Any())
            {
                List<int> productAssigneeIds = queryModel.ProductAssigneeIds
                    .Select(s => Int32.TryParse(s, out int n) ? n : (int?)null)
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .ToList();

                query = query.Where(q => productAssigneeIds.Contains(q.Person.ExternalIdentifier));
            }

            if (queryModel.ReportingCategories != null && queryModel.ReportingCategories.Any())
            {
                query = query.Where(q => queryModel.ReportingCategories.Contains(q.Product.ReportingCategory));
            }

            if (queryModel.AdvertiserIds != null && queryModel.AdvertiserIds.Any())
            {
                query = query.Where(q => queryModel.AdvertiserIds.Contains(q.Advertiser.ExternalIdentifier));
            }

            return query;
        }
    }
}
