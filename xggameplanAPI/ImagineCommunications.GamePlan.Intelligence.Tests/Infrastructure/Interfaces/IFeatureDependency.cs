using BoDi;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces
{
    public interface IFeatureDependency
    {
        void Register(IObjectContainer objectContainer);
    }
}
