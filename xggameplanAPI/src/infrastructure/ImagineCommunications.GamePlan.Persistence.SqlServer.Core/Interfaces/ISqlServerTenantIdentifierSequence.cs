using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface ISqlServerTenantIdentifierSequence : ISqlServerIdentifierSequence, ITenantIdentifierSequence
    {
    }
}
