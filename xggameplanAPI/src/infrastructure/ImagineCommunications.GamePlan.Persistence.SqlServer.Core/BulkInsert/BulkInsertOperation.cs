using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert
{
    [Flags]
    public enum BulkInsertOperation
    {
        BulkInsert = 1,
        BulkInsertOrUpdate = 2,
        BulkUpdate = 4
    }
}
