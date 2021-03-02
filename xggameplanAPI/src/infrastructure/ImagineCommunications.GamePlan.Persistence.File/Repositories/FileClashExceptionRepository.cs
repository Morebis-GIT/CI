using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileClashExceptionRepository : FileRepositoryBase, IClashExceptionRepository
    {
        public FileClashExceptionRepository(string folder) : base(folder, "clash_exception")
        {
        }

        public void Dispose()
        {
        }

        public void Add(ClashException item)
        {
            List<ClashException> items = new List<ClashException>() { item };
            InsertItems(_folder, _type, items, items.Select(i => i.Id.ToString()).ToList());
        }

        public void Add(IEnumerable<ClashException> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList());
        }

        public IEnumerable<ClashException> GetAll()
        {
            return GetAllByType<ClashException>(_folder, _type);
        }

        public int CountAll
        {
            get
            {
                return CountAll<ClashException>(_folder, _type);
            }
        }

        public ClashException Find(Guid uid)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid uid)
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<ClashExceptionModel> SearchWithDescriptions(ClashExceptionSearchQueryModel searchQuery) =>
            throw new NotImplementedException();

        public ClashException Find(int id)
        {
            return GetItemByID<ClashException>(_folder, _type, id.ToString());
        }

        public ClashExceptionModel GetWithDescriptions(int id) => throw new NotImplementedException();

        public List<ClashExceptionModel> GetWithDescriptions(IEnumerable<int> ids) => throw new NotImplementedException();

        public void Remove(int id)
        {
            DeleteItem<ClashException>(_folder, _type, id.ToString());
        }

        public IEnumerable<ClashException> FindByExternal(string externalRef)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClashException> FindByExternal(List<string> externalRefs)
        {
            throw new NotImplementedException();
        }

        public void Truncate()
        {
            DeleteAllItems<ClashException>(_folder, _type);
        }

        public IEnumerable<ClashException> Search(DateTime? dateRangeStart, DateTime? dateRangeEnd)
        {
            var items = GetAllByType<ClashException>(_folder, _type);

            if (dateRangeStart != null)
            {
                items = items.Where(p => p.StartDate.Date >= dateRangeStart.Value.Date).ToList();
            }

            if (dateRangeEnd != null)
            {
                items = items.Where(p => p.EndDate == null ||
                                         p.EndDate.Value.Date < dateRangeEnd.Value.Date.AddDays(1)).ToList();
            }

            return items;
        }

        public PagedQueryResult<ClashException> Search(ClashExceptionSearchQueryModel searchQuery)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ClashException, bool>> query)
        {
            return GetAllByType(_folder, _type, query).Count;
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Delete(ClashExceptionType fromType, ClashExceptionType toType, string fromValue, string toValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClashException> GetActive()
        {
            throw new NotImplementedException();
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            throw new NotImplementedException();
        }
    }
}
