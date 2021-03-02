using System;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Utils.DataPurging.Extensions;
using ImagineCommunications.GamePlan.Utils.DataPurging.Handlers;
using ImagineCommunications.GamePlan.Utils.DataPurging.Handlers.Campaigns;
using ImagineCommunications.GamePlan.Utils.DataPurging.Handlers.Run;
using ImagineCommunications.GamePlan.Utils.DataPurging.Handlers.Spot;
using ImagineCommunications.GamePlan.Utils.DataPurging.Helpers;
using ImagineCommunications.GamePlan.Utils.DataPurging.Hosting;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Serilog;
using Serilog.Events;
using xggameplan.common.Caching;
using xggameplan.common.Types;
using xggameplan.core.Logging;

namespace ImagineCommunications.GamePlan.Utils.DataPurging
{
    public static class Startup
    {
        public static void ConfigureSerilog(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            _ = loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .ReadFrom.Configuration(configuration)
                .Enrich.With<UtcTimeLogEventEnricher>()
                .Enrich.FromLogContext();
        }

        public static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration, CommandLineOptions cmdOptions)
        {
            var dbProvider = cmdOptions.DbProviderType ??
                             ConnectionStringHelper.RecognizeDbProviderType(cmdOptions.ConnectionString);

            switch (dbProvider)
            {
                case DbProviderType.SqlServer:
                    _ = serviceCollection
                        .AddDataPurgingHandler<RunDataPurgingHandler>(nameof(Run), Priority.High, configuration)
                        .AddDataPurgingHandler<SpotDataPurgingHandler>(nameof(Spot), Priority.High + 100, configuration)
                        .AddDataPurgingHandler<PredictionSchedulePurgingHandler>(nameof(RatingsPredictionSchedule), Priority.High + 200, configuration)
                        .AddDataPurgingHandler<CampaignDataPurgingHandler>(nameof(Campaign), Priority.High + 300, configuration);
                    break;

                default:
                    throw new InvalidOperationException($"'{dbProvider}' is unsupported.");
            }

            _ = serviceCollection
                .AddSingleton<IClock>(SystemClock.Instance)
                .AddScoped<ICache, InMemoryCache>()
                .AddHostedService<PurgingHostService>();
        }
    }
}
