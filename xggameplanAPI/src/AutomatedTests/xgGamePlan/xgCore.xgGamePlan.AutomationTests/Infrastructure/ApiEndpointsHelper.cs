using System;
using System.Collections.Generic;
using System.Linq;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Infrastructure
{
    public static class ApiEndpointsHelper
    {
        public static IEnumerable<Type> CollectApiEndpointInterfaces()
        {
            var assembly = typeof(IApiConnectivity).Assembly;
            var ns = typeof(IApiConnectivity).Namespace;
            return assembly.GetTypes().Where(t => t.IsInterface && t.Namespace == ns);
        }
    }
}
