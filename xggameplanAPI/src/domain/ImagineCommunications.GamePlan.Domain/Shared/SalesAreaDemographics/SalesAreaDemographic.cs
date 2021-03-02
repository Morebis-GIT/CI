namespace ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics
{
    public class SalesAreaDemographic
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }
        public string ExternalRef { get; set; }
        public bool Exclude { get; set; }
        public string SupplierCode { get; set; }
    }
}
