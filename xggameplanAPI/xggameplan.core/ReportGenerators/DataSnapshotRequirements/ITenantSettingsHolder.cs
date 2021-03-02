using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.core.ReportGenerators.DataSnapshotRequirements
{
    public interface ITenantSettingsHolder
    {
        TenantSettings Settings { get; }
    }
}
