using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate
{
    [Flags]
    public enum DeleteFromOptions
    {
        None = 0,
        TruncateDependent = 1,
        //RecreateConstraints = 2, //TODO: it needs to be implemented in case of complex data deletion
        UseTransaction = 4
    }
}
