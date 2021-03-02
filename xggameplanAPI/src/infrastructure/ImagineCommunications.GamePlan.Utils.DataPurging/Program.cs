using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using CommandLine.Text;
using ImagineCommunications.GamePlan.Utils.DataPurging.DependencyInjection;
using ImagineCommunications.GamePlan.Utils.DataPurging.Extensions;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ImagineCommunications.GamePlan.Utils.DataPurging
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            CommandLineOptions cmdOptions = null;
            using (var parser = new Parser(settings =>
            {
                settings.CaseInsensitiveEnumValues = true;
                settings.CaseSensitive = false;
                settings.EnableDashDash = true;
            }))
            {
                var parseResult = parser.ParseArguments<CommandLineOptions>(args);

                if (parseResult.Tag == ParserResultType.Parsed)
                {
                    cmdOptions = parseResult.MapResult(opts => opts, null);
                }
                else
                {
                    var helpText = HelpText.AutoBuild(parseResult, h => h, e => e);
                    Console.WriteLine(helpText);
                }
            }

            if (cmdOptions is null)
            {
                return;
            }

            using (var host = BuildHost(cmdOptions))
            {
                await host.RunAsync().WaitWithCancellationSuppression().ConfigureAwait(false);
            }
        }

        private static IHost BuildHost(CommandLineOptions cmdOptions)
        {
            return new HostBuilder()
                .UseSerilog((hostingContext, loggerConfiguration) =>
                    Startup.ConfigureSerilog(loggerConfiguration, hostingContext.Configuration))
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(builder =>
                    builder.AddJsonFile("config.json").AddEnvironmentVariables("DATAPURGING_"))
                .ConfigureContainer<ContainerBuilder>((context, container) =>
                    container.RegisterModule(new PurgingAutofacModule(context.Configuration, cmdOptions)))
                .ConfigureServices((hostingContext, sc) =>
                    Startup.ConfigureServices(sc, hostingContext.Configuration, cmdOptions))
                .UseConsoleLifetime(options => options.SuppressStatusMessages = true)
                .Build();
        }
    }
}
