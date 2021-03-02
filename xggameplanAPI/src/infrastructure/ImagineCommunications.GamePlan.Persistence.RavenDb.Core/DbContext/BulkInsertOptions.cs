namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext
{
    public class BulkInsertOptions : Raven.Abstractions.Data.BulkInsertOptions
    {
        public bool IsWaitForLastTaskToFinish { get; set; }
    }
}
