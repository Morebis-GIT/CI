using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings
{
    public class RSSettingsDemographicsDeliverySettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int RSSettingsDemographicsSettingId { get; set; }

        public int DaysToCampaignEnd { get; set; }

        public int UpperLimitOfOverDelivery { get; set; }

        public int LowerLimitOfOverDelivery { get; set; }
    }
}
