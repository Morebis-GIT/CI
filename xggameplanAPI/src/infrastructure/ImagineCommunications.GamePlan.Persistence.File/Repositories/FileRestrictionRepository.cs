using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileRestrictionRepository : FileRepositoryBase, IRestrictionRepository
    {
        public FileRestrictionRepository(string folder) : base(folder, "restriction")
        {

        }

        public void Add(Restriction item)
        {
            List<Restriction> items = new List<Restriction>() { item };
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList());
        }

        public void Add(IEnumerable<Restriction> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList());
        }

        public IEnumerable<Restriction> GetAll()
        {
            return GetAllByType<Restriction>(_folder, _type);
        }

        public Tuple<Restriction, RestrictionDescription> GetDesc(Guid id)
        {
            throw new NotImplementedException();
        }

        public Restriction Get(Guid id)
        {
            return GetItemByID<Restriction>(_folder, _type, id.ToString());
        }

        public Restriction Get(string externalIdentifier) => throw new NotImplementedException();

        public IEnumerable<Restriction> Get(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType)
        {
            throw new NotImplementedException();
        }

        public void Delete(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd,
            RestrictionType? restrictionType)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid uid)
        {
            DeleteItem<Restriction>(_folder, _type, uid.ToString());
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
            DeleteAllItems<Restriction>(_folder, _type);
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
