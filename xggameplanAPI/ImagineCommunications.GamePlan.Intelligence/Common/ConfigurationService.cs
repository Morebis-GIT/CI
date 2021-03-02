using ImagineCommunications.BusClient.Abstraction.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ImagineCommunications.GamePlan.Intelligence.Common
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T GetData<T>(string jsonString)
        {
            if (typeof(T).IsClass)
            {
                return _configuration.GetSection(jsonString).Get<T>();
            }
            else
            {
                return _configuration.GetValue<T>(jsonString);
            }
        }
    }
}
