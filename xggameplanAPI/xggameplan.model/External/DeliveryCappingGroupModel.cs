namespace xggameplan.Model
{
    public class DeliveryCappingGroupModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }
        public bool ApplyToPrice { get; set; }
    }
}
