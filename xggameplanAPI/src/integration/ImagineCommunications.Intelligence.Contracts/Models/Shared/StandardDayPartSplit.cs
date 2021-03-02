namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class StandardDayPartSplit
    {
        public int DayPartId { get; }
        public double Split { get; }

        public StandardDayPartSplit(int dayPartId, double split)
        {
            DayPartId = dayPartId;
            Split = split;
        }
    }
}
