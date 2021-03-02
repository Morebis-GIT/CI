using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryBreakRepository :
        MemoryRepositoryBase<Break>,
        IBreakRepository
    {
        public MemoryBreakRepository() { }

        public void Add(IEnumerable<Break> items)
        {
            InsertItems(items.ToList(), items.Select(i => i.Id.ToString()).ToList<string>());
        }

        public IEnumerable<Break> FindByExternal(string externalref)
        {
            return GetAllItems(b => b.ExternalBreakRef == externalref);
        }

        public IEnumerable<Break> FindByExternal(List<string> externalRef)
        {
            return GetAllItems(b => externalRef.Contains(b.ExternalBreakRef));
        }

        public IEnumerable<Break> Search(DateTime dateFrom, DateTime dateTo, string salesArea)
        {
            return GetAllItems(currentItem => currentItem.SalesArea == salesArea
                && currentItem.ScheduledDate >= dateFrom
                && currentItem.ScheduledDate <= dateTo);
        }

        public IEnumerable<Break> Search(DateTimeRange scheduledDatesRange, IEnumerable<string> salesAreaNames)
        {
            return GetAllItems(currentItem =>
                scheduledDatesRange.Contains(currentItem.ScheduledDate) && salesAreaNames.Contains(currentItem.SalesArea));
        }

        public IEnumerable<Break> Search(DateTime scheduledDate, string externalBreakRef, string salesArea)
        {
            return GetAllItems(currentItem =>
                currentItem.SalesArea == salesArea &&
                currentItem.ScheduledDate == scheduledDate &&
                currentItem.ExternalBreakRef == externalBreakRef);
        }

        public IEnumerable<Break> SearchByBroadcastDateRange(DateTime dateFrom, DateTime dateTo, IEnumerable<string> salesAreaNames)
        {
            return GetAllItems(currentItem => currentItem.BroadcastDate >= dateFrom
                                           && currentItem.BroadcastDate <= dateTo
                                           && salesAreaNames.Contains(currentItem.SalesArea));
        }

        public void Add(Break item)
        {
            var items = new List<Break>() { item };
            InsertItems(items, items.Select(i => i.Id.ToString()).ToList<string>());
        }

        [Obsolete("Use Get()")]
        public Break Find(Guid id) => Get(id);

        public Break Get(Guid id) => GetItemById(id.ToString());

        public IEnumerable<Break> GetAll() => GetAllItems();

        [Obsolete("Use Count()")]
        public int CountAll => GetCount();

        public int Count() => GetCount();

        [Obsolete("Use Delete()")]
        public void Remove(Guid id) => Delete(id);

        public void Delete(Guid id)
        {
            DeleteItem(id.ToString());
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            DeleteAllItems(b => ids.Contains(b.Id));
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public async Task TruncateAsync()
        {
            Truncate();
            _ = await Task.FromResult(true).ConfigureAwait(false);
        }

        public void SaveChanges() { }

        public async Task SaveChangesAsync() => await Task.FromResult(true).ConfigureAwait(false);

        public int Count(Expression<Func<Break, bool>> query) => GetCount(query);

        public void Dispose() { }
    }
}
