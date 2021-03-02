namespace ImagineCommunications.GamePlan.Domain.Generic.Interfaces
{
    public interface ICampaignKpiData
    {
        public double NominalValue { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }
    }
}
