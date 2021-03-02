using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings
{
    public class ISRDemographicSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string DemographicId { get; set; }

        public int EfficiencyThreshold { get; set; }

        public int ISRSettingId { get; set; }
    }
}
