using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    public class AgSponsorshipDayPart
    {
        [XmlElement(ElementName = "time_list.stt_time")]
        public string StartTime { get; set; }
        [XmlElement(ElementName = "time_list.end_time")]
        public string EndTime { get; set; }
        [XmlElement(ElementName = "time_list.days")]
        public int Days { get; set; }
    }
}
