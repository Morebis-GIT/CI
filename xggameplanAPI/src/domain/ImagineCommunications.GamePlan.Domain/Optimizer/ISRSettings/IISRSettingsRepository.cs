using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings
{
    public interface IISRSettingsRepository
    {
        Objects.ISRSettings Find(string salesArea);

        IEnumerable<Objects.ISRSettings> FindBySalesAreas(IEnumerable<string> salesAreas);

        List<Objects.ISRSettings> GetAll();

        void Add(IEnumerable<Objects.ISRSettings> isrSettingsList);

        void Update(Objects.ISRSettings isrSettings);

        void Delete(string salesArea);

        void SaveChanges();
    }
}
