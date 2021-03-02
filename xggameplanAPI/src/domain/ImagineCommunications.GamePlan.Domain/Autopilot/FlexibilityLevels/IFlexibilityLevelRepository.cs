using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels
{
    public interface IFlexibilityLevelRepository
    {
        void Add(FlexibilityLevel flexibilityLevel);

        void Delete(int id);

        FlexibilityLevel Get(int id);

        IEnumerable<FlexibilityLevel> GetAll();

        void SaveChanges();

        void Update(FlexibilityLevel flexibilityLevel);
    }
}
