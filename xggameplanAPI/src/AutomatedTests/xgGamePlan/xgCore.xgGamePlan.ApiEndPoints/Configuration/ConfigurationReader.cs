using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace xgCore.xgGamePlan.ApiEndPoints
{
    public partial class ConfigurationReader
    {
        private readonly ApiEndPoint _apiEndPoint;

        public ConfigurationReader(string configurationFileName)
        {
            if (string.IsNullOrWhiteSpace(configurationFileName))
            {
                throw new ArgumentNullException(nameof(configurationFileName));
            }

            string configurationRoot =
                Directory.GetParent(
                    Assembly.GetExecutingAssembly().Location
                ).FullName;

            IConfiguration _configuration = new ConfigurationBuilder()
                .SetBasePath(configurationRoot)
                .AddJsonFile(configurationFileName, optional: false, reloadOnChange: true)
                .Build();

            _apiEndPoint = _configuration.Get<ApiEndPoint>();
        }

        public string AccessToken => _apiEndPoint.AccessToken;

        public string ApiVersion => _apiEndPoint.ApiVersion;

        public string BaseAddress => _apiEndPoint.BaseAddress;
    }
}
