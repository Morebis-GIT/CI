using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRestrictionRepository : IRestrictionRepository
    {
        private readonly IDocumentSession _session;

        public RavenRestrictionRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(Restriction item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        [Obsolete("Should be called AddRange() to match .NET conventions")]
        public void Add(IEnumerable<Restriction> items)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    items
                        .ToList()
                        .ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public IEnumerable<Restriction> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Restriction>().ToList();
            }
        }

        public Tuple<Restriction, RestrictionDescription> GetDesc(Guid id)
        {
            lock (_session)
            {
                var item = _session.Query<Restriction>().FirstOrDefault(currentItem => currentItem.Uid == id);
                if (item != null)
                {
                    var desc = new RestrictionDescription();
                    if (!string.IsNullOrWhiteSpace(item.ExternalProgRef))
                    {
                        var programmes = DocumentSessionExtensions
                            .GetAll<Programmes_ByIdAndSalesAreaStartDateTime.IndexedFields,
                                Programmes_ByIdAndSalesAreaStartDateTime,
                                ProgrammeTransformer_BySearch, ProgrammeNameModel>(_session,
                                p => p.ExternalReference.Equals(item.ExternalProgRef,
                                    StringComparison.OrdinalIgnoreCase),
                                out int totalResult, null, null);

                        desc.ProgrammeDescription = programmes?.FirstOrDefault()?.ProgrammeName;
                    }
                    if (item.ProductCode != 0)
                    {
                        var products = _session.GetAll<Product>(p => p.Externalidentifier.Equals(item.ProductCode.ToString(), StringComparison.OrdinalIgnoreCase),
                            indexName: Product_BySearch.DefaultIndexName, isMapReduce: false);
                        desc.ProductDescription = products?.FirstOrDefault()?.Name;
                        desc.AdvertiserName = products?.FirstOrDefault()?.AdvertiserName;
                    }
                    if (!string.IsNullOrWhiteSpace(item.ClashCode))
                    {
                        var clashes = _session.GetAll<Clash>(p => p.Externalref.Equals(item.ClashCode.ToString(), StringComparison.OrdinalIgnoreCase),
                            indexName: Clash_BySearch.DefaultIndexName, isMapReduce: false);
                        desc.ClashDescription = clashes?.FirstOrDefault()?.Description;
                    }
                    return Tuple.Create(item, desc);
                }
                return null;
            }
        }

        public Restriction Get(Guid id)
        {
            lock (_session)
            {
                var item = _session.Query<Restriction>().FirstOrDefault(currentItem => currentItem.Uid == id);
                return item;
            }
        }

        public Restriction Get(string externalIdentifier)
        {
            lock (_session)
            {
                var item = _session.Query<Restriction>()
                    .FirstOrDefault(currentItem => currentItem.ExternalIdentifier == externalIdentifier);
                return item;
            }
        }

        public IEnumerable<Restriction> Get(List<string> externalIdentifiers) =>
            _session.GetAll<Restriction>(s => s.ExternalIdentifier.In(externalIdentifiers));

        public IEnumerable<Restriction> Get(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType)
        {
            lock (_session)
            {
                var where = new List<Expression<Func<Restriction, bool>>>();

                if (salesAreaNames != null && salesAreaNames.Any())
                {
                    if (matchAllSpecifiedSalesAreas)   // Restrictions that relate to all specified sales areas
                    {
                        where.Add(p => (p.SalesAreas == null) || (!p.SalesAreas.Any()) || (p.SalesAreas.Intersect(salesAreaNames).Count() == salesAreaNames.Count));
                    }
                    else    // Restrictions that relate to any of the specified sales areas
                    {
                        where.Add(p => (p.SalesAreas == null) || (!p.SalesAreas.Any()) || (p.SalesAreas.Intersect(salesAreaNames).Any()));
                    }
                }

                if (dateRangeStart != null)
                {
                    where.Add(p => p.StartDate >= dateRangeStart.Value.Date);
                }

                if (dateRangeEnd != null)
                {
                    //When “dateRangeEnd” is given as null, we need to ensure that we include items with null as end date.
                    //Null end date means the restrictions apply forever.
                    where.Add(p => p.EndDate < dateRangeEnd.Value.Date.AddDays(1) || p.EndDate == null);
                }

                if (restrictionType != null)
                {
                    where.Add(p => p.RestrictionType == restrictionType);
                }

                var restrictions = _session.GetAll(where.AggregateAnd());

                return restrictions;
            }
        }

        public void Delete(
            List<string> salesAreaNames,
            bool matchAllSpecifiedSalesAreas,
            DateTime? dateRangeStart,
            DateTime? dateRangeEnd,
            RestrictionType? restrictionType
            )
        {
            lock (_session)
            {
                var restrictions = Get(salesAreaNames, matchAllSpecifiedSalesAreas, dateRangeStart, dateRangeEnd, restrictionType)?.ToList();
                if (restrictions == null || !restrictions.Any())
                {
                    return;
                }

                foreach (var restriction in restrictions)
                {
                    _session.Delete<Restriction>(restriction.Id);
                }
            }
        }

        public void Delete(Guid uid)
        {
            lock (_session)
            {
                var item = Get(uid);

                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                var restrictions = _session.GetAll<Restriction>(s => s.Uid.In(ids));

                foreach (var restriction in restrictions)
                {
                    _session.Delete(restriction);
                }
            }
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            lock (_session)
            {
                var restrictions = _session.GetAll<Restriction>(c => c.ExternalIdentifier.In(externalRefs));

                foreach (var restriction in restrictions)
                {
                    _session.Delete(restriction);
                }
            }
        }

        public void Truncate() =>
            _session.TryDelete("Raven/DocumentsByEntityName", "Restrictions");

        public PagedQueryResult<Tuple<Restriction, RestrictionDescription>> Get(RestrictionSearchQueryModel query)
        {
            lock (_session)
            {
                var restrictions = _session.GetAll<Restriction>();
                if (query.SalesAreaNames != null && query.SalesAreaNames.Any())
                {
                    if (query.MatchAllSpecifiedSalesAreas) // Restrictions that relate to all specified sales areas
                    {
                        restrictions = restrictions?.Where(p => (p.SalesAreas == null) || (!p.SalesAreas.Any()) ||
                                                                (p.SalesAreas.Intersect(query.SalesAreaNames).Count() ==
                                                                 query.SalesAreaNames.Count))?.ToList();
                    }
                    else // Restrictions that relate to any of the specified sales areas
                    {
                        restrictions = restrictions?.Where(p => (p.SalesAreas == null) || (!p.SalesAreas.Any()) ||
                                                                (p.SalesAreas.Intersect(query.SalesAreaNames).Any()))
                            ?.ToList();
                    }
                }
                if (query.DateRangeStart != null && query.DateRangeEnd != null)
                {
                    restrictions = restrictions?.Where(c => (c.StartDate.Date >= query.DateRangeStart.Value.Date &&
                                                             c.StartDate.Date < query.DateRangeEnd.Value.Date
                                                                 .AddDays(1)) ||
                                                            (c.EndDate != null && c.EndDate.Value.Date >=
                                                             query.DateRangeStart.Value.Date &&
                                                             c.EndDate.Value.Date < query.DateRangeEnd.Value.Date
                                                                 .AddDays(1)) ||
                                                            (c.StartDate.Date < query.DateRangeEnd.Value.Date
                                                                 .AddDays(1) &&
                                                             (c.EndDate == null || c.EndDate.Value.Date >=
                                                              query.DateRangeEnd.Value.Date))).ToList();
                }
                else
                {
                    if (query.DateRangeStart != null && query.DateRangeEnd == null)
                    {
                        restrictions = restrictions?.Where(c => (c.StartDate.Date >= query.DateRangeStart.Value.Date) ||
                                                                (c.StartDate.Date < query.DateRangeStart.Value.Date
                                                                     .AddDays(1) &&
                                                                 (c.EndDate == null ||
                                                                  c.EndDate.Value.Date >= query.DateRangeStart.Value
                                                                      .Date))).ToList();
                    }
                    else
                    {
                        if (query.DateRangeStart == null && query.DateRangeEnd != null)
                        {
                            restrictions = restrictions?
                                .Where(c => (c.EndDate != null &&
                                             c.EndDate.Value.Date < query.DateRangeEnd.Value.Date.AddDays(1)) ||
                                            (c.StartDate.Date < query.DateRangeEnd.Value.Date.AddDays(1) &&
                                             (c.EndDate == null ||
                                              c.EndDate.Value.Date >= query.DateRangeEnd.Value.Date)))
                                .ToList();
                        }
                    }
                }

                if (query.RestrictionType != null)
                {
                    restrictions = restrictions?.Where(p => p.RestrictionType == query.RestrictionType)?.ToList();
                }

                if (restrictions == null || !restrictions.Any())
                {
                    return new PagedQueryResult<Tuple<Restriction, RestrictionDescription>>(0, null);
                }

                var sortedItems = restrictions.Sort(query.OrderBy.ToString(), query.OrderDirection.ToString().ToLower())
                    ?.ToList();

                if (query.Skip != null)
                {
                    sortedItems = sortedItems?.Skip(query.Skip.Value).ToList();
                }

                if (query.Top != null)
                {
                    sortedItems = sortedItems?.Take(query.Top.Value).ToList();
                }

                if (sortedItems == null || !sortedItems.Any())
                {
                    return new PagedQueryResult<Tuple<Restriction, RestrictionDescription>>(0, null);
                }

                List<ProgrammeNameModel> programmes = null;
                List<Product> products = null;
                List<Clash> clashes = null;
                var externalProgRefs = sortedItems.Where(i => !string.IsNullOrWhiteSpace(i?.ExternalProgRef))
                    .Select(i => i.ExternalProgRef)
                    .Distinct(StringComparer.OrdinalIgnoreCase)?.ToList();
                if (externalProgRefs.Any())
                {
                    programmes = DocumentSessionExtensions
                        .GetAll<Programmes_ByIdAndSalesAreaStartDateTime.IndexedFields,
                            Programmes_ByIdAndSalesAreaStartDateTime,
                            ProgrammeTransformer_BySearch, ProgrammeNameModel>(_session,
                            p => p.ExternalReference.In(externalProgRefs),
                            out int totalResult, null, null);
                }
                var productCodes = sortedItems.Where(i => i.ProductCode != 0).Select(i => i.ProductCode.ToString())
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                if (productCodes.Any())
                {
                    products = _session.GetAll<Product>(p => p.Externalidentifier.In(productCodes),
                        indexName: Product_BySearch.DefaultIndexName, isMapReduce: false);
                }
                var clashCodes = sortedItems.Where(i => !string.IsNullOrWhiteSpace(i?.ClashCode))
                    .Select(i => i.ClashCode.ToString())
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                if (clashCodes.Any())
                {
                    clashes = _session.GetAll<Clash>(p => p.Externalref.In(clashCodes),
                        indexName: Clash_BySearch.DefaultIndexName, isMapReduce: false);
                }
                var items = (from res in sortedItems
                             let programme = !string.IsNullOrWhiteSpace(res.ExternalProgRef)
                        ? programmes.EmptyIfNull()
                            .FirstOrDefault(p => p.ExternalReference.Equals(res.ExternalProgRef,
                                StringComparison.OrdinalIgnoreCase))
                        : null
                             let product = res.ProductCode != 0
                                 ? products.EmptyIfNull()
                                     .FirstOrDefault(p => p.Externalidentifier.Equals(res.ProductCode.ToString(),
                                         StringComparison.OrdinalIgnoreCase))
                                 : null
                             let clash = !string.IsNullOrWhiteSpace(res.ClashCode)
                                 ? clashes.EmptyIfNull()
                                     .FirstOrDefault(
                                         c => c.Externalref.Equals(res.ClashCode, StringComparison.OrdinalIgnoreCase))
                                 : null

                             select Tuple.Create(res, new RestrictionDescription()
                             {
                                 ClashDescription = clash?.Description,
                                 ProductDescription = product?.Name,
                                 ProgrammeDescription = programme?.ProgrammeName
                             })).ToList();
                return new PagedQueryResult<Tuple<Restriction, RestrictionDescription>>(restrictions.Count, items);
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void UpdateRange(IEnumerable<Restriction> restrictions)
        {
            if (restrictions is null || !restrictions.Any())
            {
                return;
            }

            lock (_session)
            {
                foreach (var restriction in restrictions)
                {
                    if (restriction != null)
                    {
                        _session.Store(restriction);
                    }
                }
            }
        }
    }
}
