using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Autopilot.Settings
{
    public interface IAutopilotSettingsRepository
    {
        void Add(AutopilotSettings autopilotSettings);

        void Delete(int id);

        AutopilotSettings Get(int id);

        IEnumerable<AutopilotSettings> GetAll();

        AutopilotSettings GetDefault();

        void SaveChanges();

        void Update(AutopilotSettings autopilotSettings);
    }
}
