using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileBreakRepository
        : FileRepositoryBase, IBreakRepository
    {
        public FileBreakRepository(string folder) : base(folder, "break")
        {
        }

        public void Add(IEnumerable<Break> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList<string>());
        }

        public IEnumerable<Break> FindByExternal(string externalref)
        {
            return GetAllByType<Break>(_folder, _type, b => b.ExternalBreakRef == externalref);
        }

        public IEnumerable<Break> FindByExternal(List<string> externalRef)
        {
            return GetAllByType<Break>(_folder, _type, b => externalRef.Contains(b.ExternalBreakRef));
        }

        public IEnumerable<Break> Search(DateTime dateFrom, DateTime dateTo, string salesArea)
        {
            return GetAllByType<Break>(_folder, _type, currentItem => currentItem.SalesArea == salesArea && currentItem.ScheduledDate >= dateFrom && currentItem.ScheduledDate <= dateTo);
        }

        public IEnumerable<Break> Search(DateTimeRange scheduledDatesRange, IEnumerable<string> salesAreaNames)
        {
            return GetAllByType<Break>(_folder, _type, currentItem =>
                scheduledDatesRange.Contains(currentItem.ScheduledDate) && salesAreaNames.Contains(currentItem.SalesArea));
        }

        public IEnumerable<Break> Search(DateTime scheduledDate, string externalBreakRef, string salesArea)
        {
            return GetAllByType<Break>(_folder, _type, currentItem =>
                currentItem.SalesArea == salesArea &&
                currentItem.ScheduledDate == scheduledDate &&
                currentItem.ExternalBreakRef == externalBreakRef);
        }

        public IEnumerable<Break> SearchByBroadcastDateRange(DateTime dateFrom, DateTime dateTo, IEnumerable<string> salesAreaNames)
        {
            return GetAllByType<Break>(_folder, _type, currentItem =>
                currentItem.BroadcastDate >= dateFrom
                && currentItem.BroadcastDate <= dateTo
                && salesAreaNames.Contains(currentItem.SalesArea));
        }

        public void Add(Break item)
        {
            var items = new List<Break>() { item };
            InsertItems(_folder, _type, items, items.Select(i => i.Id.ToString()).ToList<string>());
        }

        [Obsolete("Use Get()")]
        public Break Find(Guid id) => Get(id);

        public Break Get(Guid id)
        {
            return GetItemByID<Break>(_folder, _type, id.ToString());
        }

        public IEnumerable<Break> GetAll() => GetAllByType<Break>(_folder, _type);

        [Obsolete("Use Count()")]
        public int CountAll => Count();

        public int Count() => CountAll(_folder, _type);

        [Obsolete("Use Delete()")]
        public void Remove(Guid id) => Delete(id);

        public void Delete(Guid id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            DeleteAllItems<Break>(_folder, _type, b => ids.Contains(b.Id));
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate()
        {
            DeleteAllItems(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }

        public void SaveChanges()
        {
        }

        public Task SaveChangesAsync() => Task.CompletedTask;

        public void Dispose()
        {
        }

        public int Count(Expression<Func<Break, bool>> query) =>
            GetAllByType(_folder, _type, query).Count;
    }
}
