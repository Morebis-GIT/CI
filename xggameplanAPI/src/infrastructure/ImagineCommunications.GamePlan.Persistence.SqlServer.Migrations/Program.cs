using System;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration;
using Serilog;
using xggameplan.core.Logging;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations
{
    public class Program
    {
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("./config.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.With<UtcTimeLogEventEnricher>()
                .ReadFrom.Configuration(_configuration, "serilog")
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                var exception = eventArgs.ExceptionObject as Exception;

                Log.Error(exception.Message);

                Log.Debug(exception, exception.Message);

                Environment.Exit(-1);
            };

            using (var parser = new Parser(settings =>
            {
                settings.CaseInsensitiveEnumValues = true;
                settings.CaseSensitive = false;
                settings.EnableDashDash = true;
            }))
            {
                var parseResult = parser.ParseArguments<CommandLineOptions>(args)
                    .WithParsed(RunMigrations);

                parseResult.WithNotParsed(errs =>
                {
                    var helpText = HelpText.AutoBuild(parseResult, h => h, e => e);
                    Console.WriteLine(helpText);
                });
            }

            Log.CloseAndFlush();
        }

        private static void RunMigrations(CommandLineOptions options)
        {
            Log.Debug("Tool has been run with the following options: {@options}", options);

            var migrator = new EfCoreMigrator(_configuration, Log.Logger);

            migrator.Migrate(options);
        }
    }
}
