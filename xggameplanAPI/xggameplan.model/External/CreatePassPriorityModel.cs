namespace xggameplan.Model
{
    public class CreatePassPriorityModel
    {
        public int PassId { get; set; } //this needs to be confirmed
        public string PassName { get; set; }
        public int Priority { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
