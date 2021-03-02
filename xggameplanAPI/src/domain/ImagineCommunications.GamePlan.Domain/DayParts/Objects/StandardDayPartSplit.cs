namespace ImagineCommunications.GamePlan.Domain.DayParts.Objects
{
    public class StandardDayPartSplit
    {
        public int DayPartId { get; set; }
        public double Split { get; set; }

        public StandardDayPartSplit(int dayPartId, double split)
        {
            DayPartId = dayPartId;
            Split = split;
        }
    }
}
