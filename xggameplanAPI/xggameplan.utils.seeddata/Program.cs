using System;
using CommandLine;
using CommandLine.Text;
using xggameplan.utils.seeddata.Migration;
using xggameplan.utils.seeddata.Seeding;

namespace xggameplan.utils.seeddata
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine("Something went wrong. See log file for more details.");
            };

            using (var parser = new Parser(settings =>
            {
                settings.CaseInsensitiveEnumValues = true;
                settings.CaseSensitive = false;
                settings.EnableDashDash = true;
            }))
            {
                var parseResult = parser.ParseArguments<SeedOptions, MigrationOptions>(args)
                    .WithParsed<SeedOptions>(RunSeeding)
                    .WithParsed<MigrationOptions>(RunMigration);

                _ = parseResult.WithNotParsed(errs =>
                  {
                      var helpText = HelpText.AutoBuild(parseResult, h => h, e => e);
                      Console.WriteLine(helpText);
                  });
            }
        }

        private static void RunMigration(MigrationOptions opts)
        {
            var migrator = new Migrator(opts);
            migrator.Run();
        }

        private static void RunSeeding(SeedOptions opts)
        {
            var seeder = new Seeder(opts);
            var success = seeder.Run(out var message);

            if (success)
            {
                Console.WriteLine("Data seeding has been successful");
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
