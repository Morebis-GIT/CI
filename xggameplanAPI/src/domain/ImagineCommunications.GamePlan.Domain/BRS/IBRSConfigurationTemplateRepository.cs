using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.BRS
{
    public interface IBRSConfigurationTemplateRepository
    {
        void Add(BRSConfigurationTemplate item);
        void Update(BRSConfigurationTemplate item);
        void Delete(int id);
        BRSConfigurationTemplate Get(int id);
        BRSConfigurationTemplate GetByName(string name);
        IEnumerable<BRSConfigurationTemplate> GetAll();
        void SaveChanges();
        BRSConfigurationTemplate GetDefault();
        bool ChangeDefaultConfiguration(int id);
        int Count();
        bool Exists(int id);
    }
}
