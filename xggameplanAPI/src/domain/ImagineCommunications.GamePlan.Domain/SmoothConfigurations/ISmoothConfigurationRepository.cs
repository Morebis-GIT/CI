using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations
{
    public interface ISmoothConfigurationRepository
    {
        SmoothConfiguration GetById(int id);

        void Add(SmoothConfiguration smoothConfiguration);

        void Update(SmoothConfiguration smoothConfiguration);

        void SaveChanges();
        void Truncate();
    }
}
