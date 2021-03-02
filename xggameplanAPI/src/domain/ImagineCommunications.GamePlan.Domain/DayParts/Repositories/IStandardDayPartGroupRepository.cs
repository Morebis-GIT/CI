using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;

namespace ImagineCommunications.GamePlan.Domain.DayParts.Repositories
{
    public interface IStandardDayPartGroupRepository
    {
        IEnumerable<StandardDayPartGroup> GetAll();
        StandardDayPartGroup Get(int id);
        void AddRange(IEnumerable<StandardDayPartGroup> dayPartGroups);
        void SaveChanges();
        void Truncate();
    }
}
