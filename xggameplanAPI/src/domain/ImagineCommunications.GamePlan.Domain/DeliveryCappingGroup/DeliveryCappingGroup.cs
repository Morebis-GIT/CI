using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup
{
    public class DeliveryCappingGroup: IIntIdentifier
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }
        public bool ApplyToPrice { get; set; }
    }
}
