using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.common.Extensions;
using xggameplan.common.Services;
using xggameplan.core.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenCampaignRepository : ICampaignRepository
    {
        private const int MAX_CLAUSE_COUNT = 1000;

        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;
        private readonly IIdentityGenerator _identityGenerator;

        public RavenCampaignRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync, IIdentityGenerator identityGenerator)
        {
            _session = session;
            _sessionAsync = sessionAsync;
            _identityGenerator = identityGenerator;
        }

        [Obsolete("Use the Get() method")]
        public Campaign Find(Guid id) => Get(id);

        [Obsolete("Use the Delete() method")]
        public void Remove(Guid uid) => Delete(uid);

        [Obsolete("Should be called AddRange() to match .NET conventions")]
        public void Add(IEnumerable<Campaign> items)
        {
            var campaigns = items.ToList();
            Stack<int> campaignIds = null;

            // Generate campaign custom Ids. Lock not really necessary but do it
            // for robustness
            using (MachineLock.Create("xggameplan.postcampaigns", TimeSpan.FromMinutes(10)))
            {
                campaignIds = new Stack<int>(_identityGenerator.GetIdentities<CampaignNoIdentity>(campaigns.Count)
                    .Select(x => x.Id));
            }

            foreach (var campaign in campaigns)
            {
                campaign.UpdateDerivedKPIs();

                if (campaignIds.Count == 0)
                {
                    throw new InvalidDataException($"Failed to generate CustomId for campaign {campaign.Name}");
                }
                campaign.CustomId = campaignIds.Pop();
            }

            var options = new BulkInsertOptions() { OverwriteExisting = true };
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
            {
                campaigns.ForEach(item => bulkInsert.Store(item));
            }
        }

        public Campaign Add(Campaign item)
        {
            lock (_session)
            {
                if (item.CustomId == 0)
                {
                    item.CustomId = FindCustomId();
                }

                item.UpdateDerivedKPIs();

                _session.Store(item);

                return item;
            }
        }

        public void Update(Campaign campaign)
        {
            lock (_session)
            {
                campaign.UpdateDerivedKPIs();
                _session.Store(campaign);
            }
        }

        private int FindCustomId()
        {
            var idlist = _session.GetAll<Campaign>()?.Select(s => s.CustomId).ToList();
            return idlist != null && idlist.Any() ? idlist.Max() + 33 : 1;
        }

        public Campaign Get(Guid id) => _session.Load<Campaign>(id);

        public IEnumerable<Campaign> Find(List<Guid> uids)
        {
            return _session.GetAll<Campaign>(c => c.Id.In(uids));
        }

        public IEnumerable<Campaign> FindByRef(string externalref)
        {
            var items = _session.Query<Campaign>().Where(c => c.ExternalId == externalref);
            return items;
        }

        public IEnumerable<Campaign> GetByGroup(string group)
        {
            var items = _session.Query<Campaign>().Where(c => c.CampaignGroup == group);
            return items;
        }

        public IEnumerable<Campaign> FindByRefs(List<string> externalRefs)
        {
            List<Campaign> items = new List<Campaign>();
            var groups = GroupElementsForClause(externalRefs);
            foreach (var group in groups)
            {
                items.AddRange(
                    _session.GetAll<Campaign>(c => c.ExternalId.In(group.ToList()),
                        indexName: Campaigns_BySearch.DefaultIndexName, isMapReduce: false));
            }
            return items;
        }

        public IEnumerable<Campaign> FindMissingCampaignsFromGroup(List<string> externalRefs, List<string> campaignGroup)
        {
            List<Campaign> campaigns = new List<Campaign>();

            var groups = GroupElementsForClause(campaignGroup);
            foreach (var group in groups)
            {
                campaigns.AddRange(_session.GetAll<Campaign>(c => c.CampaignGroup.In(group.ToList())));
            }
            var items = campaigns.Where(r => !externalRefs.Contains(r.ExternalId)).ToList();
            return items;
        }

        public IEnumerable<Campaign> GetAll() => _session.GetAll<Campaign>();

        public IEnumerable<CampaignReducedModel> GetAllFlat() =>
            DocumentSessionExtensions.GetAll<Campaign, Campaigns_BySearch, CampaignReducedModel_Transformer, CampaignReducedModel>(_session, null, out var total, null, null);

        public IEnumerable<CampaignNameModel> FindNameByRefs(ICollection<string> externalRefs)
        {
            var distinctExternalRefs = externalRefs.Distinct().ToList();
            var result = new List<CampaignNameModel>();
            for (int i = 0, page = 0; i < distinctExternalRefs.Count; i += MAX_CLAUSE_COUNT, page++)
            {
                var ids = distinctExternalRefs.Skip(MAX_CLAUSE_COUNT * page).Take(MAX_CLAUSE_COUNT).ToArray();
                result.AddRange(_session.GetAllUsingProjection<Campaign, Campaigns_BySearch, CampaignNameModel>(x => x.ExternalId.In(ids)));
            }

            return result;
        }

        public IEnumerable<string> GetBusinessTypes() =>
            _session.GetAllWithSelect<Campaign, string>(x => x.BusinessType != null && x.BusinessType != string.Empty,
                x => x.BusinessType, Campaigns_BySearch.DefaultIndexName, false).Distinct().ToList();

        /// <summary>
        /// To Get a list of all Active Campaign's ExternalIds
        /// </summary>
        /// <returns>
        /// A list of all <see cref="IEnumerable{string}"/> containing all
        /// Active Campaign's ExternalIds
        /// </returns>
        public IEnumerable<string> GetAllActiveExternalIds()
        {
            return _session.GetAllWithSelect<Campaign, string>(c => c.ExternalId != null &&
                                                                    c.Status.Equals("A", StringComparison.OrdinalIgnoreCase),
                                                               c => c.ExternalId,
                                                               Campaigns_BySearch.DefaultIndexName, false);
        }

        /// <summary>
        /// To Get a list of Campaigns including the product details by applying
        /// the filters based on the values in the supplied queryModel parameter
        /// </summary>
        /// <param name="queryModel">The Query model <see cref="CampaignSearchQueryModel"/></param>
        /// <returns>The Campaigns as <see cref="PagedQueryResult{CampaignWithProductFlatModel}"/></returns>
        public PagedQueryResult<CampaignWithProductFlatModel> GetWithProduct(CampaignSearchQueryModel queryModel)
        {
            //get a list of ExternalRef and Name as Key and Value from Demographic document
            var demographics = _session.GetAll<Demographic>()
                                       .Select(a => new KeyValuePair<string, string>(a.ExternalRef, a.Name)).ToList();

            //get a list of ExternalRef and Description as Key and Value from Clash document
            var clashes = _session.GetAll<Clash>()
                                  .Select(a => new KeyValuePair<string, string>(a.Externalref, a.Description)).ToList();

            //get a list of Product Externalidentifiers filtered by description search
            var descriptionProductQuery = BuildDescriptionProductQuery(queryModel, clashes);
            var filteredOrElseProductExternalidentifiers = GetFilteredProductExternalidentifiers(descriptionProductQuery);

            //get a list of Product Externalidentifiers filtered by search
            var andAlsoProductQuery = BuildAndAlsoProductQuery(queryModel);
            var filteredAndAlsoProductExternalidentifiers = GetFilteredProductExternalidentifiers(andAlsoProductQuery);

            //get a list of Campaigns filtered by search and also those matching the filteredProductExternalidentifiers
            var filteredCampaigns = GetFilteredCampaigns(
                queryModel,
                demographics,
                clashes,
                filteredOrElseProductExternalidentifiers,
                filteredAndAlsoProductExternalidentifiers);
            var totalCount = (filteredCampaigns != null) ? filteredCampaigns.Count : 0;

            var result = new List<CampaignWithProductFlatModel>();
            if (totalCount > 0 && filteredCampaigns != null && filteredCampaigns.Any())
            {
                var campaignProductExternalidentifiers = filteredCampaigns.Select(c => c.Product).Distinct().ToList();

                //get a list of Products filtered by campaignProductExternalidentifiers
                var filteredProducts = GetFilteredProducts(campaignProductExternalidentifiers);
                //create a list of CampaignWithProductFlatModel
                var campaignModels = ConvertToCampaignWithProductFlatModel(filteredCampaigns, filteredProducts,
                                                                          demographics, clashes);
                if (queryModel != null && queryModel.GroupByGroupName)
                {
                    campaignModels = GroupByCampaignGroup(campaignModels, out totalCount);
                }

                if (campaignModels != null)
                {
                    result = campaignModels.ApplySortingAndPaging(queryModel).ToList();
                }
            }

            return new PagedQueryResult<CampaignWithProductFlatModel>(totalCount, result);
        }

        private List<CampaignWithProductFlatModel> GroupByCampaignGroup(IEnumerable<CampaignWithProductFlatModel> items, out int count)
        {
            if (items == null)
            {
                count = 0;
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
            count = result.Count;
            return result;
        }

        /// <summary>
        /// Get a list of products filtered by the supplied list of Product
        /// External Identifiers.
        /// </summary>
        /// <param name="productExternalIds">
        /// A collection of Product External Identifiers <see cref="IReadOnlyCollection{string}"/>
        /// </param>
        /// <returns>
        /// A list of Products matching the supplied list of Product External
        /// Identifiers <see cref="List{Product}"/>
        /// </returns>
        private List<Product> GetFilteredProducts(IReadOnlyCollection<string> productExternalIds)
        {
            if (productExternalIds is null)
            {
                return new List<Product>();
            }

            var productExternalIdCount = productExternalIds.Count;

            if (productExternalIdCount == 0)
            {
                return new List<Product>();
            }

            const int maxBatchSize = 500;

            if (productExternalIdCount <= maxBatchSize)
            {
                return _session.GetAll<Product>(a => a.Externalidentifier.In(productExternalIds));
            }

            var results = new List<Product>();
            var amountLeftToProcess = productExternalIdCount;
            var iteration = 0;

            do
            {
                IEnumerable<string> productIdBatch = productExternalIds
                    .Skip(iteration * maxBatchSize)
                    .Take(maxBatchSize);

                results.AddRange(
                    _session.GetAll<Product>(a => a.Externalidentifier.In(productIdBatch))
                    );

                amountLeftToProcess -= maxBatchSize;
                iteration++;
            } while (amountLeftToProcess > 0);

            return results;
        }

        /// <summary>
        /// Get a list of Campaigns based on the search criteria in the supplied queryModel
        /// </summary>
        /// <param name="queryModel">
        /// The queryModel <see cref="CampaignSearchQueryModel"/> containing the
        /// the search criteria
        /// </param>
        /// <param name="demographics">
        /// The list of ExternalRef and Name as Key and Value from Demographic document
        /// </param>
        /// <param name="clashes">
        /// The list of ExternalRef and Description as Key and Value from Clash document
        /// </param>
        /// <param name="filteredProductExternalidentifiers">
        /// the list of Product Externalidentifiers
        /// </param>
        /// <returns>
        /// A list of Campaigns <see cref="List{Campaign}"/> matching the search criteria
        /// </returns>
        private List<Campaign> GetFilteredCampaigns(CampaignSearchQueryModel queryModel,
                                                    IEnumerable<KeyValuePair<string, string>> demographics,
                                                    IEnumerable<KeyValuePair<string, string>> clashes,
                                                    IEnumerable<string> filteredProductExternalidentifiers,
                                                    IEnumerable<string> andAlsoFilteredProductExternalidentifiers)
        {
            var filteredCampaigns = new List<Campaign>();

            //build the Campaign query based on the search
            var campaignQuery = BuildCampaignQuery(queryModel, demographics, clashes, filteredProductExternalidentifiers, andAlsoFilteredProductExternalidentifiers);
            if (campaignQuery != null)
            {
                using (var campaignStream = _session.Advanced.Stream<Campaign>(campaignQuery))
                {
                    while (campaignStream.MoveNext())
                    {
                        filteredCampaigns.Add(campaignStream.Current.Document);
                    }
                }
            }

            return filteredCampaigns;
        }

        /// <summary> To Build a Query <see cref="IDocumentQuery{Campaign}"/>
        /// using the supplied parameter values </summary> <param
        /// name="queryModel">The query model <see
        /// cref="CampaignSearchQueryModel"/> </param> <param
        /// name="demographics">The list of ExternalRef and Name as key and
        /// value from Demographic document <see
        /// cref="IEnumerable{KeyValuePair<string, string>}"/> </param> <param
        /// name="clashes">The list of ExternalRef and Description as key and
        /// value from Clash document <see
        /// cref="IEnumerable{KeyValuePair<string, string>}"/> </param>
        /// <returns><see cref="IDocumentQuery{Campaign}"/></returns>
        private IDocumentQuery<Campaign> BuildCampaignQuery(CampaignSearchQueryModel queryModel,
                                                            IEnumerable<KeyValuePair<string, string>> demographics,
                                                            IEnumerable<KeyValuePair<string, string>> clashes,
                                                            IEnumerable<string> filteredProductExternalidentifiers,
                                                            IEnumerable<string> andAlsoFilteredProductExternalidentifiers)
        {
            var campaignQuery = _session.Advanced.DocumentQuery<Campaign>(indexName: Campaigns_BySearch.DefaultIndexName, isMapReduce: false);

            if (queryModel != null)
            {
                //filter by status
                campaignQuery = campaignQuery.ContainsAny(c => c.Status, queryModel.Status.ConvertToSearchKeys().Cast<object>());

                //filter by start date and end date
                if (queryModel.StartDate != default(DateTime) && queryModel.EndDate != default(DateTime) &&
                    queryModel.StartDate <= queryModel.EndDate)
                {
                    campaignQuery = campaignQuery.AndAlso().OpenSubclause()
                                                 .WhereLessThanOrEqual(x => x.StartDateTime, queryModel.EndDate.AddDays(1))
                                                 .AndAlso()
                                                 .WhereGreaterThanOrEqual(x => x.EndDateTime, queryModel.StartDate.Date)
                                                 .CloseSubclause();
                }

                //filter by description
                if (!string.IsNullOrWhiteSpace(queryModel.Description))
                {
                    queryModel.Description = queryModel.Description.Trim().ToLowerInvariant();

                    //apply search for CampaignGroup, Name, ExternalId, BusinessType
                    campaignQuery = campaignQuery.AndAlso().OpenSubclause()
                    .WhereEquals(c => c.CampaignGroup, queryModel.Description)
                    .OrElse()
                    .WhereEquals(c => c.Name, queryModel.Description)
                    .OrElse()
                    .WhereEquals(c => c.ExternalId, queryModel.Description)
                    .OrElse()
                    .WhereEquals(c => c.BusinessType, queryModel.Description)
                    .OrElse()
                    .WhereStartsWith(c => c.CampaignGroup, queryModel.Description)
                    .OrElse()
                    .WhereStartsWith(c => c.Name, queryModel.Description)
                    .OrElse()
                    .WhereStartsWith(c => c.ExternalId, queryModel.Description)
                    .OrElse()
                    .WhereStartsWith(c => c.BusinessType, queryModel.Description)
                    .OrElse()
                    .WhereEndsWith(c => c.CampaignGroup, queryModel.Description)
                    .OrElse()
                    .WhereEndsWith(c => c.Name, queryModel.Description)
                    .OrElse()
                    .WhereEndsWith(c => c.ExternalId, queryModel.Description)
                    .OrElse()
                    .WhereEndsWith(c => c.BusinessType, queryModel.Description)
                    .OrElse()
                    .Search(c => c.CampaignGroup,
                                 $"*{queryModel.Description}*",
                                 escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards)
                    .OrElse()
                    .Search(c => c.Name,
                                 $"*{queryModel.Description}*",
                                 escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards)
                    .OrElse()
                    .Search(c => c.ExternalId,
                                 $"*{queryModel.Description}*",
                                 escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards)
                    .OrElse()
                    .Search(c => c.BusinessType,
                                 $"*{queryModel.Description}*",
                                 escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);

                    //apply search for Demographics
                    var filteredDemographics = demographics != null ?
                                               demographics.Where(x => x.Value.ToLowerInvariant().Contains(queryModel.Description)).ToList() :
                                               null;
                    if (filteredDemographics != null && filteredDemographics.Any())
                    {
                        campaignQuery = campaignQuery.OrElse()
                                     .WhereIn(x => x.DemoGraphic, filteredDemographics.Select(x => x.Key).ToList());
                    }

                    //filter by Product Externalidentifiers
                    if (filteredProductExternalidentifiers != null && filteredProductExternalidentifiers.Any())
                    {
                        campaignQuery = campaignQuery.OrElse()
                                          .WhereIn(x => x.Product, filteredProductExternalidentifiers.ToList());
                    }

                    campaignQuery = campaignQuery.CloseSubclause();
                }

                // filter by business type
                if (queryModel.BusinessTypes != null && queryModel.BusinessTypes.Any())
                {
                    campaignQuery = campaignQuery.AndAlso()
                        .WhereIn(x => x.BusinessType, queryModel.BusinessTypes);
                }

                // filter by campaign external identifier
                if (queryModel.CampaignIds != null && queryModel.CampaignIds.Any())
                {
                    campaignQuery = campaignQuery.AndAlso()
                        .WhereIn(x => x.ExternalId, queryModel.CampaignIds);
                }

                // filter by product id
                if (queryModel.ProductIds != null && queryModel.ProductIds.Any())
                {
                    campaignQuery = campaignQuery.AndAlso()
                        .WhereIn(x => x.Product, queryModel.ProductIds);
                }

                // filter by product
                if (IsAnyProductFilterInUse(queryModel))
                {
                    campaignQuery = campaignQuery.AndAlso()
                        .WhereIn(x => x.Product, andAlsoFilteredProductExternalidentifiers ?? new List<string>());
                }
            }

            return campaignQuery;
        }

        /// <summary>
        /// Get a List of Product Externalidentifiers based on the search
        /// criteria in the supplied queryModel
        /// </summary>
        /// <param name="productQuery">
        /// The product query containing the the search criteria
        /// </param>
        /// <returns>A List of Product Externalidentifiers <see cref="List{string}"/></returns>
        private List<string> GetFilteredProductExternalidentifiers(IDocumentQuery<Product> productQuery)
        {
            List<string> filteredProductExternalidentifiers = null;

            if (productQuery != null)
            {
                var filteredProducts = new List<Product>();

                using (var productStream = _session.Advanced.Stream<Product>(productQuery))
                {
                    while (productStream.MoveNext())
                    {
                        filteredProducts.Add(productStream.Current.Document);
                    }
                }

                if (filteredProducts.Any())
                {
                    filteredProductExternalidentifiers = filteredProducts.Select(p => p.Externalidentifier)
                                                                         .Distinct().ToList();
                }
            }

            return filteredProductExternalidentifiers;
        }

        /// <summary> To Build a Query <see cref="IDocumentQuery{Product}"/>
        /// using the supplied parameter values for Or Else scenario </summary>
        /// <param name="queryModel">The query model <see
        /// cref="CampaignSearchQueryModel"/> </param> <see
        /// cref="IEnumerable{KeyValuePair<string, string>}"/> </param> <param
        /// name="clashes">The list of ExternalRef and Description as key and
        /// value from Clash document <see
        /// cref="IEnumerable{KeyValuePair<string, string>}"/> </param>
        /// <returns><see cref="IDocumentQuery{Campaign}"/></returns>
        private IDocumentQuery<Product> BuildDescriptionProductQuery(CampaignSearchQueryModel queryModel,
                                                          IEnumerable<KeyValuePair<string, string>> clashes)
        {
            if (queryModel != null)
            {
                //filter by description
                if (!string.IsNullOrWhiteSpace(queryModel.Description))
                {
                    queryModel.Description = queryModel.Description.Trim().ToLowerInvariant();

                    var productQuery = _session.Advanced.DocumentQuery<Product>(indexName: Product_BySearch.DefaultIndexName,
                                                                                isMapReduce: false);
                    //apply search for AdvertiserName, AgencyName, Name
                    productQuery = productQuery.OpenSubclause()
                                  .WhereEquals(c => c.AdvertiserName, queryModel.Description)
                                  .OrElse()
                                  .WhereEquals(c => c.AgencyName, queryModel.Description)
                                  .OrElse()
                                  .WhereEquals(c => c.Name, queryModel.Description)
                                  .OrElse()

                                  .WhereStartsWith(c => c.AdvertiserName, queryModel.Description)
                                  .OrElse()
                                  .WhereStartsWith(c => c.AgencyName, queryModel.Description)
                                  .OrElse()
                                  .WhereStartsWith(c => c.Name, queryModel.Description)
                                  .OrElse()

                                  .WhereEndsWith(c => c.AdvertiserName, queryModel.Description)
                                  .OrElse()
                                  .WhereEndsWith(c => c.AgencyName, queryModel.Description)
                                  .OrElse()
                                  .WhereEndsWith(c => c.Name, queryModel.Description)
                                  .OrElse()
                                  .Search(c => c.AdvertiserName,
                                               $"*{queryModel.Description}*",
                                               EscapeQueryOptions.AllowAllWildcards)
                                  .OrElse()
                                  .Search(c => c.AgencyName,
                                               $"*{queryModel.Description}*",
                                               EscapeQueryOptions.AllowAllWildcards)
                                  .OrElse()
                                  .Search(c => c.Name,
                                               $"*{queryModel.Description}*",
                                               EscapeQueryOptions.AllowAllWildcards);

                    //apply search for ClashCodes
                    var filteredClashCodes = clashes != null ?
                                             clashes.Where(a => a.Key.ToLowerInvariant().Contains(queryModel.Description)).Select(c => c.Key).ToList() :
                                             new List<string>();

                    //apply search for ClashDescription
                    var filteredClashCodesByClashDesc = clashes != null ?
                                                        clashes.Where(a => a.Value.ToLowerInvariant().Contains(queryModel.Description)).Select(c => c.Key).ToList() :
                                                        new List<string>();

                    if (filteredClashCodesByClashDesc.Any() || filteredClashCodes.Any())
                    {
                        var clashCodes = filteredClashCodes.Union(filteredClashCodesByClashDesc).Distinct().ToList();
                        productQuery = productQuery.OrElse().WhereIn(a => a.ClashCode, clashCodes);
                    }

                    productQuery = productQuery.CloseSubclause();
                    return productQuery;
                }
            }

            return null;
        }

        /// <summary>
        /// To Build a Query <see cref="IDocumentQuery{Product}"/> using the
        /// supplied parameter values for And Also scenario
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private IDocumentQuery<Product> BuildAndAlsoProductQuery(CampaignSearchQueryModel queryModel)
        {
            if (queryModel is null)
            {
                return null;
            }

            IDocumentQuery<Product> productQuery = null;

            // filter by clash code
            if (queryModel.ClashCodes != null && queryModel.ClashCodes.Any())
            {
                productQuery = productQuery ?? _session.Advanced.DocumentQuery<Product>(indexName: Product_BySearch.DefaultIndexName,
                                                                                isMapReduce: false);

                productQuery = productQuery.AndAlso()
                    .WhereIn(x => x.ClashCode, queryModel.ClashCodes);
            }

            // filter by agency id
            if (queryModel.AgencyIds != null && queryModel.AgencyIds.Any())
            {
                productQuery = productQuery ?? _session.Advanced.DocumentQuery<Product>(indexName: Product_BySearch.DefaultIndexName,
                                                                                isMapReduce: false);

                productQuery = productQuery.AndAlso()
                    .WhereIn(x => x.AgencyIdentifier, queryModel.AgencyIds);
            }

            // filter by media sales group (agency group)
            if (queryModel.MediaSalesGroupIds != null && queryModel.MediaSalesGroupIds.Any())
            {
                productQuery = productQuery ?? _session.Advanced.DocumentQuery<Product>(indexName: Product_BySearch.DefaultIndexName,
                                                                                isMapReduce: false);

                productQuery = productQuery.AndAlso()
                    .WhereIn(x => x.AgencyGroup.Code, queryModel.MediaSalesGroupIds);
            }

            // filter by product assignee (person/sales exec)
            if (queryModel.ProductAssigneeIds != null && queryModel.ProductAssigneeIds.Any())
            {
                productQuery = productQuery ?? _session.Advanced.DocumentQuery<Product>(indexName: Product_BySearch.DefaultIndexName,
                                                                                isMapReduce: false);

                List<int> productAssigneeIds = queryModel.ProductAssigneeIds
                        .Select(s => Int32.TryParse(s, out int n) ? n : (int?)null)
                        .Where(n => n.HasValue)
                        .Select(n => n.Value)
                        .ToList();

                productQuery = productQuery.AndAlso()
                    .WhereIn(x => x.SalesExecutive.Identifier, productAssigneeIds);
            }

            // filter by reporting categories
            if (queryModel.ReportingCategories != null && queryModel.ReportingCategories.Any())
            {
                productQuery = productQuery ?? _session.Advanced.DocumentQuery<Product>(indexName: Product_BySearch.DefaultIndexName,
                                                                                isMapReduce: false);

                productQuery.AndAlso().OpenSubclause();

                productQuery.WhereEquals(p => p.ReportingCategory, queryModel.ReportingCategories.First());

                foreach (var reportingCategory in queryModel.ReportingCategories.Skip(1))
                {
                    productQuery.OrElse()
                        .WhereEquals(p => p.ReportingCategory, reportingCategory);
                }

                productQuery.CloseSubclause();
            }

            // filter by advertiser ids
            if (queryModel.AdvertiserIds != null && queryModel.AdvertiserIds.Any())
            {
                productQuery = productQuery ?? _session.Advanced.DocumentQuery<Product>(indexName: Product_BySearch.DefaultIndexName,
                                                                                isMapReduce: false);

                productQuery = productQuery.AndAlso()
                    .WhereIn(x => x.AdvertiserIdentifier, queryModel.AdvertiserIds);
            }

            return productQuery;
        }

        /// <summary> To convert the supplied list of <see
        /// cref="IEnumerable{CampaignWithProduct.Result}"/> into <see
        /// cref="IList{CampaignWithProductFlatModel}"/> using the supplied
        /// parameter values </summary> <param name="campaigns">The list of <see
        /// cref="IEnumerable{CampaignWithProduct.Result}"/></param> <param
        /// name="demographics">The list of ExternalRef and Name as key and
        /// value from Demographic document <see
        /// cref="IEnumerable{KeyValuePair<string, string>}"/> </param> <param
        /// name="clashes">The list of ExternalRef and Description as key and
        /// value from Clash document <see
        /// cref="IEnumerable{KeyValuePair<string, string>}"/> </param>
        /// <returns>A list of <see
        /// cref="IEnumerable{CampaignWithProductFlatModel}"/> when the supplied
        /// campaigns parameter contains data/></returns> <returns>null when the
        /// supplied campaigns parameter contains no data/></returns>
        private IEnumerable<CampaignWithProductFlatModel> ConvertToCampaignWithProductFlatModel(IEnumerable<Campaign> campaigns,
                                                                            IEnumerable<Product> products,
                                                                            IEnumerable<KeyValuePair<string, string>> demographics,
                                                                            IEnumerable<KeyValuePair<string, string>> clashes)
        {
            if (campaigns == null || !campaigns.Any())
            {
                return null;
            }

            return campaigns.Select(x =>
            {
                var product = products.FirstOrDefault(p => p.Externalidentifier == x.Product);
                return new CampaignWithProductFlatModel
                {
                    ActualRatings = x.ActualRatings,
                    AdvertiserName = product?.AdvertiserName,
                    AgencyName = product?.AgencyName,
                    ProductAssigneeName = product?.SalesExecutive?.Name,
                    MediaGroup = product?.AgencyGroup is null ? null : new AgencyGroupModel
                    {
                        Code = product.AgencyGroup.Code,
                        ShortName = product.AgencyGroup.ShortName
                    },
                    ReportingCategory = product?.ReportingCategory,
                    BusinessType = x.BusinessType,
                    DeliveryType = x.DeliveryType,
                    CampaignGroup = x.CampaignGroup,
                    Demographic = demographics.Where(d => d.Key == x.DemoGraphic).Select(a => a.Value).FirstOrDefault(),
                    EndDateTime = x.EndDateTime,
                    ExternalId = x.ExternalId,
                    IncludeOptimisation = x.IncludeOptimisation,
                    TargetZeroRatedBreaks = x.TargetZeroRatedBreaks,
                    IncludeRightSizer = MapToIncludeRightSizer(x.RightSizerLevel),
                    InefficientSpotRemoval = x.InefficientSpotRemoval,
                    IsPercentage = x.IsPercentage,
                    Name = x.Name,
                    ProductExternalRef = x.Product,
                    ProductName = product?.Name,
                    RevenueBudget = x.RevenueBudget,
                    StartDateTime = x.StartDateTime,
                    Status = GetStatusText(x.Status), //changing A or C to Active or Cancelled
                    TargetRatings = x.TargetRatings,
                    Uid = x.Id,
                    CustomId = x.CustomId,
                    DefaultCampaignPassPriority = ValidateCampaignPassPriority(x.CampaignPassPriority)
                        ? x.CampaignPassPriority
                        : (int)PassPriorityType.Exclude,
                    ClashCode = product?.ClashCode,
                    ClashDescription = clashes.Where(a =>
                        a.Key == products.Where(p => p.Externalidentifier == x.Product)
                            .Select(p => p.ClashCode).FirstOrDefault()).Select(a => a.Value).FirstOrDefault(),
                    StopBooking = x.StopBooking,
                    TargetXP = x.TargetXP,
                    RevenueBooked = x.RevenueBooked,
                    CreationDate = x.CreationDate,
                    AutomatedBooked = x.AutomatedBooked,
                    TopTail = x.TopTail,
                    Spots = x.Spots,
                    CampaignPaybacks = x.CampaignPaybacks,
                    ActiveLength = x.ActiveLength,
                    RatingsDifferenceExcludingPayback = x.RatingsDifferenceExcludingPayback,
                    ValueDifference = x.ValueDifference,
                    ValueDifferenceExcludingPayback = x.ValueDifferenceExcludingPayback,
                    AchievedPercentageTargetRatings = x.AchievedPercentageTargetRatings,
                    AchievedPercentageRevenueBudget = x.AchievedPercentageRevenueBudget
                };
            });
        }

        private bool IsAnyProductFilterInUse(CampaignSearchQueryModel queryModel)
        {
            return (queryModel.ClashCodes != null && queryModel.ClashCodes.Any()) ||
                (queryModel.AgencyIds != null && queryModel.AgencyIds.Any()) ||
                (queryModel.MediaSalesGroupIds != null && queryModel.MediaSalesGroupIds.Any()) ||
                (queryModel.ProductAssigneeIds != null && queryModel.ProductAssigneeIds.Any()) ||
                (queryModel.ReportingCategories != null && queryModel.ReportingCategories.Any()) ||
                (queryModel.AdvertiserIds != null && queryModel.AdvertiserIds.Any());
        }

        private string AggregateActiveLength(IEnumerable<string> items)
        {
            var distinctActiveLength = items
                .Where(e => !string.IsNullOrEmpty(e))
                .SelectMany(e => e.Split('/'))
                .Select(e => int.Parse(e))
                .Distinct()
                .ToList();

            distinctActiveLength.Sort();

            return string.Join("/", distinctActiveLength);
        }

        private static bool ValidateCampaignPassPriority(int campaignPassPriority)
        {
            return campaignPassPriority >= (int)PassPriorityType.Exclude
                   && campaignPassPriority <= (int)PassPriorityType.Include;
        }

        private RightSizerLevel? ParseRightSizerLevelFromIndexQuery(CampaignWithProduct.Result campaign)
        {
            if (!(campaign.IncludeRightSizer ?? default))
            {
                return null;
            }

            string rightSizerLevelValue = campaign.RightSizerLevel;

            if (string.IsNullOrWhiteSpace(rightSizerLevelValue))
            {
                throw new ArgumentException($"RightSizerLevel value is empty. Campaign Id: {campaign.Id}");
            }

            if (rightSizerLevelValue.TryGetValueFromString<RightSizerLevel>(out RightSizerLevel result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"RightSizerLevel value '{rightSizerLevelValue}' is not valid. Campaign Id: {campaign.Id}");
            }
        }

        /// <summary>
        /// Gets the Uid contained in the supplied dbDocumentIdContainingUid parameter
        /// </summary>
        /// <param name="dbDocumentIdContainingUid">
        /// The dbDocumentId Containing the Uid. The dbDocumentId will contain a
        /// '/' followed by the Uid
        /// </param>
        /// <returns>
        /// A Guid when the supplied dbDocumentIdContainingUid parameter
        /// contains a valid Guid else an empty Guid.
        /// </returns>
        private Guid GetUidFromDbDocumentId(string dbDocumentIdContainingUid)
        {
            var charToSplit = '/';
            int charToSplitFoundAtIndex;
            if (string.IsNullOrWhiteSpace(dbDocumentIdContainingUid) ||
                (charToSplitFoundAtIndex = dbDocumentIdContainingUid.IndexOf(charToSplit)) == -1)
            { return Guid.Empty; }

            Guid result;
            return Guid.TryParse(dbDocumentIdContainingUid.Substring(charToSplitFoundAtIndex + 1), out result) ? result : Guid.Empty;
        }

        private string GetStatusText(string statusCode)
        {
            if (string.IsNullOrWhiteSpace(statusCode)) { return statusCode; }
            switch (statusCode.ToUpperInvariant())
            {
                case "A":
                    return "Active";

                case "C":
                    return "Cancelled";

                default:
                    return statusCode;
            }
        }

        private IncludeRightSizer MapToIncludeRightSizer(RightSizerLevel? rightSizer)
        {
            return rightSizer != null
                ? rightSizer.Value == RightSizerLevel.CampaignLevel
                    ? IncludeRightSizer.CampaignLevel
                    : IncludeRightSizer.DetailLevel
                : IncludeRightSizer.No;
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<Campaign>();
                }
            }
        }

        public IEnumerable<Campaign> GetAllScenarioUI()
        {
            // Passing a LINQ expression to RavenDB that compares two double
            // properties causes 'Could not understand expression' exception.
            return _session
                .GetAll<Campaign>(c =>
                    c.SalesAreaCampaignTarget != null &&
                    c.SalesAreaCampaignTarget.Any() &&
                    !c.Status.Equals("C", StringComparison.OrdinalIgnoreCase)
                )
                .Where(c => c.ActualRatings >= c.TargetRatings)
                .ToList();
        }

        public IEnumerable<Campaign> GetAllActive()
        {
            return GetAll().Where(c =>
                (c.DeliveryType == CampaignDeliveryType.Spot ? c.TargetRatings >= default(decimal) : c.TargetZeroRatedBreaks || c.TargetRatings > default(decimal)) &&
                c.SalesAreaCampaignTarget != null &&
                c.SalesAreaCampaignTarget.Any() &&
                !c.Status.Equals("C", StringComparison.OrdinalIgnoreCase)
            );
        }

        public int CountAllActive => GetAllFlat().Count(x => x.IsActive);

        public void Delete(Guid uid)
        {
            _session.Delete<Campaign>(uid);
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate()
        {
            _session.TryDelete("Raven/DocumentsByEntityName", "Campaigns");
        }

        public async Task TruncateAsync()
        {
            const int maximumTimeoutSeconds = 180;
            const int retryMillisecondDelay = 100;
            const int maximumNumberOfRetries = 100;

            var maximumSecondsWaitingForNonStaleIndexes = TimeSpan.FromSeconds(maximumTimeoutSeconds);
            int remainingRetries = maximumNumberOfRetries;
            bool retry = false;
            var startTime = DateTime.UtcNow;

            do
            {
                retry = false;

                try
                {
                    var operation = await _sessionAsync.Advanced
                        .DeleteByIndexAsync<Campaign, Campaigns_ById>(ForceDelete())
                        .ConfigureAwait(false);

                    await operation
                        .WaitForCompletionAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (CanRetry(ex))
                {
                    remainingRetries--;
                    retry = true;

                    await Task.Delay(retryMillisecondDelay)
                        .ConfigureAwait(false);

                    continue;
                }
                catch (Exception ex) when (remainingRetries == 0)
                {
                    throw new RepositoryException(
                        $"Deleting all Campaign documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Campaign documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
                        ex
                    );
                }
            } while (retry);

            bool IndexIsStale(Exception ex) => ex.Message.Contains("index is stale");

            bool MaximumTimeToWaitForIndexesExceeded() =>
                DateTime.UtcNow - startTime > maximumSecondsWaitingForNonStaleIndexes;

            bool CanRetry(Exception ex) =>
                IndexIsStale(ex) &&
                remainingRetries > 0 &&
                !MaximumTimeToWaitForIndexesExceeded();

            Expression<Func<Campaign, bool>> ForceDelete() =>
                campaign => campaign.Id != Guid.Empty;
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        /// <summary>
        /// To avoid 'Exceeded maximum clause count in the query.' exception in
        /// case when elements in clause greater than 1000 we will group them
        /// for several request
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private static IEnumerable<IGrouping<int, string>> GroupElementsForClause(List<string> elements)
        {
            return elements
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / MAX_CLAUSE_COUNT, x => x.Item);
        }

        /// <summary>
        /// To Delete Campaigns with ExternalId matching the supplied list of campaignExternalIds
        /// </summary>
        /// <param name="campaignExternalIds">
        /// The list of <see cref="IEnumerable{string}"/> containing the
        /// campaign ExternalIds to Delete
        /// </param>
        public void Delete(IEnumerable<string> campaignExternalIds)
        {
            if (campaignExternalIds?.Any() == true)
            {
                var campaignsToDelete = FindByRefs(campaignExternalIds.Where(e => !string.IsNullOrWhiteSpace(e))
                                                                      .Select(e => e.Trim()).ToList());
                if (campaignsToDelete.Any())
                {
                    foreach (var campaign in campaignsToDelete.Where(c => c != null))
                    {
                        lock (_session)
                        {
                            _session.Delete<Campaign>(campaign.Id);
                        }
                    }
                }
            }
        }
    }
}
