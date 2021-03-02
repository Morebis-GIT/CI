namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class AutopilotRule
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int RuleTypeId { get; set; }
        public int FlexibilityLevelId { get; set; }
        public bool Enabled { get; set; }
        public int LoosenBit { get; set; }
        public int LoosenLot { get; set; }
        public int TightenBit { get; set; }
        public int TightenLot { get; set; }
    }
}
