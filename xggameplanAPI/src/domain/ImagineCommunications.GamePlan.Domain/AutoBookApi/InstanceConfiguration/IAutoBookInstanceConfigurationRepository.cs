using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration
{
    public interface IAutoBookInstanceConfigurationRepository
    {
        void Add(IEnumerable<AutoBookInstanceConfiguration> instanceConfigurations);

        List<AutoBookInstanceConfiguration> GetAll();

        AutoBookInstanceConfiguration Get(int id);

        void SaveChanges();
    }
}
