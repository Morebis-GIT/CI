using System;
using System.Linq;
using AutoMapper;

namespace xggameplan.Configuration
{
    /// <summary>
    /// Configures AutoMapper, so AutoMapperMapper class can be used to map objects to objects.
    /// </summary>
    public static class AutoMapperConfig
    {
        /// <summary>
        /// Loads all classes derived from Profile class and runs its configurations.
        /// </summary>
        public static void SetupAutoMapper(IMapperConfigurationExpression configuration)
        {
            var profiles = typeof(AutoMapperConfig).Assembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(AutoMapper.Profile)));
            foreach (var profile in profiles)
            {
                configuration.AddProfile(Activator.CreateInstance(profile) as AutoMapper.Profile);
            }
        }
    }
}
