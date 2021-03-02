using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.Gameplan.Synchronization
{
    public class SynchronizationServiceConfigurator
    {
        private readonly SynchronizationServicesConfiguration _configuration;

        public SynchronizationServiceConfigurator(IServiceCollection services)
        {
            _configuration = new SynchronizationServicesConfiguration();

            var serviceDescriptor = new ServiceDescriptor(typeof(SynchronizationServicesConfiguration), _configuration);
            services.Add(serviceDescriptor);
        }

        public void AddService(int id, int? maxConcurrencyLevel = null)
        {
            var configuration = new SynchronizationServiceConfiguration(id, maxConcurrencyLevel);
            _configuration.Add(configuration);
        }
    }
}
