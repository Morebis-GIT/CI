namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection
{
    public class SqlServerDbContextRegistrationFeatures
    {
        public bool Audit { get; set; } = true;
        public bool BulkInsert { get; set; } = true;
        public bool Logging { get; set; }
        public int? CommandTimeout { get; set; }
    }
}
