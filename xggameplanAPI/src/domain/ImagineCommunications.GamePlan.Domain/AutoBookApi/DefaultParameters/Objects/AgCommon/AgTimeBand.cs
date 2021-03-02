using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgTimeBand
    {
        [XmlElement(ElementName = "time_data.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "time_data.end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "time_data.days")]
        public int Days { get; set; }
    }
}
