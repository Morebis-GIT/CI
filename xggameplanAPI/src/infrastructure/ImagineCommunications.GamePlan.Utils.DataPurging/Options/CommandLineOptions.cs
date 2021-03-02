using System.Collections.Generic;
using CommandLine;
using ImagineCommunications.GamePlan.Domain.Shared.System;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Options
{
    public class CommandLineOptions
    {
        [Option('c', "connection", Required = true, HelpText = "Connection string")]
        public string ConnectionString { get; set; }

        [Option('e', "entities", Required = false, HelpText = "The list of entities which need to be processed")]
        public IEnumerable<string> Entities { get; set; }

        /// <summary>
        /// It's hardcoded to SqlServer but can be changed to be an option in the future
        /// in order to support not SqlServer only.
        /// </summary>
        public DbProviderType? DbProviderType { get; set; } = Domain.Shared.System.DbProviderType.SqlServer;
    }
}
