using System;
using System.Reflection;
using BoDi;
using xgCore.xgGamePlan.ApiEndPoints;
using xgCore.xgGamePlan.ApiEndPoints.Utils;

namespace xgCore.xgGamePlan.AutomationTests.Extensions
{
    public static class ObjectContainerExtensions
    {
        private static readonly MethodInfo ApiEndpointRegisterMethod =
            typeof(ObjectContainerExtensions).GetMethod(nameof(RegisterApiEndpointInternal),
                BindingFlags.Static | BindingFlags.NonPublic);

        private static void RegisterApiEndpointInternal<T>(IObjectContainer objectContainer)
        {
            objectContainer.RegisterFactoryAs<T>(oc =>
            {
                var configReader = oc.Resolve<ConfigurationReader>();
                return ApiClientFactory.GetEndPoint<T>(new Uri(configReader.BaseAddress), configReader.AccessToken);
            });
        }

        public static void RegisterApiEndpoints(this IObjectContainer objectContainer, params Type[] apiEndpointInterfaces)
        {
            foreach (var apiEndpointInterface in apiEndpointInterfaces)
            {
                ApiEndpointRegisterMethod.MakeGenericMethod(apiEndpointInterface)
                    .Invoke(null, new object[] {objectContainer});
            }
        }
    }
}
