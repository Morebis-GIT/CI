using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings
{
    public interface IRSSettingsRepository
    {
        Objects.RSSettings Find(string salesArea);

        IEnumerable<Objects.RSSettings> FindBySalesAreas(IEnumerable<string> salesAreas);

        List<Objects.RSSettings> GetAll();

        void Add(IEnumerable<Objects.RSSettings> rsSettingsList);

        void Update(Objects.RSSettings rsSettings);

        void Delete(string salesArea);

        void SaveChanges();
    }
}
