using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using xggameplan.core.Logging;

namespace xggameplan.utils.seeddata.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterSerilog(this ContainerBuilder builder, string configFileName)
        {
            _ = builder.Register(context => new LoggerConfiguration()
                      .Enrich.With<UtcTimeLogEventEnricher>()
                      .ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile(configFileName).Build())
                      .CreateLogger())
                .As<ILogger>()
                .InstancePerLifetimeScope()
                .OnRelease(logger => logger.Dispose());
        }
    }
}
