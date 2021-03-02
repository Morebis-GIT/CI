using CommandLine;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace xggameplan.utils.seeddata.Migration
{
    [Verb("migrate", HelpText = "Migrate data from RavenDb to SqlServer")]
    public class MigrationOptions
    {
        [Option('s', "source", Required = true, HelpText = "Source database connection string")]
        public string ConnectionStringFrom { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Destination database connection string")]
        public string ConnectionStringTo { get; set; }

        [Option('t', "db", Required = true, HelpText = "Database type - master or tenant")]
        public DatabaseType DatabaseType { get; set; }
    }
}
