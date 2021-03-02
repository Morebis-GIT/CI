using CommandLine;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System;

namespace xggameplan.utils.seeddata.Seeding
{
    [Verb("seed", HelpText = "Seed the specified database by data from the specified folder")]
    public class SeedOptions
    {
        [Option('c', "connection", Required = true, HelpText = "Connection string")]
        public string ConnectionString { get; set; }

        [Option('t', "db", Required = true, HelpText = "Database type - master or tenant")]
        public DatabaseType DatabaseType { get; set; }

        [Option('p', "provider", Required = false, HelpText = "Database provider type - RavenDb or SqlServer")]
        public DbProviderType? DbProviderType { get; set; }

        [Option('d', "data", Required = true, HelpText = "Folder with seed data")]
        public string SeedDataFolder { get; set; }

        [Option('r', "replace", Required = false, HelpText = "Replaces existing data if it exists", Default = false)]
        public bool ReplaceExistingData { get; set; }
    }
}
