using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations
{
    public interface IAWSInstanceConfigurationRepository
    {
        void Add(IEnumerable<AWSInstanceConfiguration> items);

        List<AWSInstanceConfiguration> GetAll();

        AWSInstanceConfiguration Get(int id);

        void SaveChanges();
    }
}
