namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgDayPartLength
    {
        public int CampaignNo { get; set; }
        public int SalesAreaNo { get; set; }
        public string StartDate { get; set; }
        public int DayPartType { get; set; }
        public int DayPartNo { get; set; }
        public long SpotLength { get; set; }
        public int MultipartNumber { get; set; }
        public AgRequirement AgPartLengthRequirement { get; set; }
    }
}
