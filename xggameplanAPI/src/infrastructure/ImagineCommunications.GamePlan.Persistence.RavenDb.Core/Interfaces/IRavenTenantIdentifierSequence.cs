using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces
{
    public interface IRavenTenantIdentifierSequence : IRavenIdentifierSequence, ITenantIdentifierSequence
    {
    }
}
