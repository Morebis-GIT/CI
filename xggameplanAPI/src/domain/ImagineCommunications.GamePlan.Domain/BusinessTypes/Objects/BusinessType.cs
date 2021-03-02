namespace ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects
{
    public class BusinessType
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IncludeConversionIndex { get; set; }
    }
}
