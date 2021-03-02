using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryRestrictionRepository :
        MemoryRepositoryBase<Restriction>,
        IRestrictionRepository
    {
        public MemoryRestrictionRepository()
        {
        }

        public void Add(Restriction item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public void Add(IEnumerable<Restriction> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public IEnumerable<Restriction> GetAll() => GetAllItems();

        public Tuple<Restriction, RestrictionDescription> GetDesc(Guid id)
        {
            throw new NotImplementedException();
        }

        public Restriction Get(Guid id)
        {
            return GetItemById(id.ToString());
        }

        public Restriction Get(string externalIdentifier) => throw new NotImplementedException();

        public IEnumerable<Restriction> Get(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType)
        {
            throw new NotImplementedException();
        }

        public void Delete(
            List<string> salesAreaNames,
            bool matchAllSpecifiedSalesAreas,
            DateTime? dateRangeStart,
            DateTime? dateRangeEnd,
            RestrictionType? restrictionType)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid uid)
        {
            DeleteItem(uid.ToString());
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            throw new NotImplementedException();
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public void SaveChanges()
        {
        }

        public PagedQueryResult<Tuple<Restriction, RestrictionDescription>> Get(RestrictionSearchQueryModel query)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public IEnumerable<Restriction> Get(List<string> externalIdentifiers)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Restriction> restrictions)
        {
            throw new NotImplementedException();
        }
    }
}
