using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.GamePlan.Intelligence.Common
{
    public interface IDependencyModule
    {
        void Register(IServiceCollection services);
    }
}
