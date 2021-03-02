using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgTimeSlice
    {
        /* StartDay and EndDay calculation - If the DOW pattern is from MON to TUE and then THU to SUN then there will be 2 time slice records where
           start day and end day will be 1 to 2 and 4 to 7 */

        [XmlElement(ElementName = "dcdt_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "dcdt_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "dcdt_data.dcdp_no")]
        public int DayPartNo { get; set; }

        [XmlElement(ElementName = "dcdt_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "dcdt_data.stt_day")]
        public int StartDay { get; set; }

        [XmlElement(ElementName = "dcdt_data.end_day")]
        public int EndDay { get; set; }

        [XmlElement(ElementName = "dcdt_data.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "dcdt_data.end_time")]
        public string EndTime { get; set; }
    }
}
