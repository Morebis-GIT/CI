using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Tenants
{

    public class DatabaseProviderConfiguration
    {
        private DatabaseProviderConfiguration()
        {
        }

        public DbProviderType Provider { get; private set; }

        [Obsolete("Remove it after Sql conversion is complete")]
        public string ConfigurationJson { get; private set ; }

        private string _connectionString;

        public string ConnectionString {
            get => _connectionString ?? ( ConfigurationJson != null ? GetConnectionStringFromJsonConfiguration(ConfigurationJson) : null);
                set => _connectionString = value; }

        public static DatabaseProviderConfiguration Create(DbProviderType provider,string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            
            return new DatabaseProviderConfiguration()
            {
               ConnectionString = connectionString,
               Provider = provider
            };
        }

        [Obsolete]
        public static DatabaseProviderConfiguration CreateFromConfiguration(DbProviderType provider, string configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            return Create(provider,GetConnectionStringFromJsonConfiguration(configuration));
        }

        private static string GetConnectionStringFromJsonConfiguration(string configurationJson)
        {
            string connectionString;
            try
            {
                var jconfig = JObject.Parse(configurationJson);
                connectionString = jconfig["connectionString"]?.Value<string>() ?? string.Empty;
            }
            catch (JsonReaderException ex)
            {
                return string.Empty;
                //throw new ArgumentException($"{nameof(configurationJson)} is not valid Json");
            }
            
            return connectionString;
        }
    }
}
