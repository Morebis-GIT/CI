using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Domain.Breaks
{
    public interface IBreakRepository
        : IRepository<Break>
    {
#warning Push this up to IRepository<T>

        int Count();

#warning Push this up to IRepository<T>

        void Delete(Guid id);

        void Delete(IEnumerable<Guid> ids);

#pragma warning disable CA1716 // Identifiers should not match keywords
#warning Push this up to IRepository<T>

        Break Get(Guid id);

#pragma warning restore CA1716 // Identifiers should not match keywords

        IEnumerable<Break> Search(DateTime datefrom, DateTime dateto, string salesarea);

        IEnumerable<Break> Search(DateTimeRange scheduledDatesRange, IEnumerable<string> salesAreaNames);

        IEnumerable<Break> Search(DateTime scheduledDate, string externalBreakRef, string salesArea);

        IEnumerable<Break> SearchByBroadcastDateRange(DateTime dateFrom, DateTime dateTo, IEnumerable<string> salesAreaNames);

#warning Push this up to IRepository<T>

        void SaveChanges();

#warning Push this up to IRepository<T>

        Task SaveChangesAsync();

#warning Push this up to IRepository<T>

        Task TruncateAsync();
    }
}
