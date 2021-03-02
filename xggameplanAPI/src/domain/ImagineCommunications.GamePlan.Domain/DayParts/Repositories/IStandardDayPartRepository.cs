using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;

namespace ImagineCommunications.GamePlan.Domain.DayParts.Repositories
{
    public interface IStandardDayPartRepository
    {
        IEnumerable<StandardDayPart> GetAll();
        StandardDayPart Get(int id);
        void AddRange(IEnumerable<StandardDayPart> dayParts);
        IEnumerable<StandardDayPart> FindByExternal(List<int> externalIds);
        void SaveChanges();
        void Truncate();
    }
}
