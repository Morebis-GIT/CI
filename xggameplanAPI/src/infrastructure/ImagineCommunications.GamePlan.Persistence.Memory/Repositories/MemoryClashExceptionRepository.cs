﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryClashExceptionRepository :
        MemoryRepositoryBase<ClashException>,
        IClashExceptionRepository
    {
        public MemoryClashExceptionRepository()
        {
        }

        public void Dispose()
        {
        }

        public void Add(ClashException item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public void Add(IEnumerable<ClashException> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public IEnumerable<ClashException> GetAll() => GetAllItems();

        public int CountAll => GetCount();

        public int Count(Expression<Func<ClashException, bool>> query) => GetCount(query);

        public ClashException Find(Guid uid)
        {
            throw new NotImplementedException();
        }

        public ClashException Find(int id) => GetItemById(id.ToString());

        public void Remove(Guid uid)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            DeleteItem(id.ToString());
        }

        public void Delete(ClashExceptionType fromType, ClashExceptionType toType, string fromValue, string toValue)
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<ClashExceptionModel> SearchWithDescriptions(ClashExceptionSearchQueryModel searchQuery) =>
                            throw new NotImplementedException();

        public ClashExceptionModel GetWithDescriptions(int id) => throw new NotImplementedException();

        public List<ClashExceptionModel> GetWithDescriptions(IEnumerable<int> ids) => throw new NotImplementedException();

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
            DeleteAllItems();
        }

        public IEnumerable<ClashException> Search(DateTime? dateRangeStart, DateTime? dateRangeEnd)
        {
            var items = GetAllItems();

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

        public void SaveChanges()
        {
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
