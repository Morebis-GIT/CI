using System.Web.Http;
using Microsoft.Extensions.Configuration;

namespace xggameplan.Configuration
{
    /// <summary>
    /// Provides methods for configuring Autofac dependency resolver.
    /// </summary>
    public static class AutofacConfig
    {
        /// <summary>
        /// Registers dependencies from this and other assemblies and sets Autofac as a dependency resolver for WebApi Controllers and Filters.
        /// </summary>
        /// <param name="httpConfiguration">Http configuration which will be configured</param>
        /// <param name="applicationConfiguration">Configuration specifying which modules should be loaded</param>
        public static void SetupAutofac(HttpConfiguration httpConfiguration, IConfiguration applicationConfiguration) =>
            ApplicationModulesLoader.Configure(httpConfiguration, applicationConfiguration);
    }
}
