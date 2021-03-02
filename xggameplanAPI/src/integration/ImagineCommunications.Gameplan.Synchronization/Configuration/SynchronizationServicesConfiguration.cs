using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Synchronization
{
    public sealed class SynchronizationServicesConfiguration
    {
        private readonly Dictionary<int, SynchronizationServiceConfiguration> _services;

        public SynchronizationServicesConfiguration()
        {
            _services = new Dictionary<int, SynchronizationServiceConfiguration>();
        }

        public SynchronizationServicesConfiguration Add(SynchronizationServiceConfiguration service)
        {
            _services.Add(service.Id, service);

            return this;
        }

        public SynchronizationServicesConfiguration Add(int id, int? maxConcurrencyLevel = null)
        {
            Add(new SynchronizationServiceConfiguration(id, maxConcurrencyLevel));

            return this;
        }

        public bool TryGet(int id, out SynchronizationServiceConfiguration service)
        {
            return _services.TryGetValue(id, out service);
        }
    }
}
