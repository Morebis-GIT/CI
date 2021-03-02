using CommandLine;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations
{
    public class CommandLineOptions
    {
        [Option('d', "database", Required = false, HelpText = "Database connection string (by default connection string from config.json is used)")]
        public string ConnectionString { get; set; }

        [Option('c', "context", Required = false, HelpText = "Name of DbContext to run migrations againgst (by default tenant db context is used)")]
        public string DbContext { get; set; }
    }
}
