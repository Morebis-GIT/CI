namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BusinessTypes
{
    public class BusinessType
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IncludeConversionIndex { get; set; }
    }
}
