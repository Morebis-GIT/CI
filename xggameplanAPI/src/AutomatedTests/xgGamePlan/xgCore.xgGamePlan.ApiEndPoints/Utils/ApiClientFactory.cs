using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Refit;

namespace xgCore.xgGamePlan.ApiEndPoints.Utils
{
    public static class ApiClientFactory
    {
        public static T GetEndPoint<T>(Uri baseAddress, string token)
        {
            var httpClient = new HttpClient(new AuthenticatedHttpClientHandler(() => Task.FromResult(token)))
            {
                BaseAddress = baseAddress
            };

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new JsonContentSerializer(serializerSettings)
            };

            return RestService.For<T>(httpClient, refitSettings);
        }
    }
}
