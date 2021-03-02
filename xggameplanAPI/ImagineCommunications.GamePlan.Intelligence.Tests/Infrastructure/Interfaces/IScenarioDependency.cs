using BoDi;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces
{
    public interface IScenarioDependency
    {
        void Register(IObjectContainer objectContainer);
    }
}
