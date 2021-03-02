using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgPassTimeSlice
    {
        [XmlElement(ElementName = "time_list.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "time_list.end_time")]
        public string EndTime { get; set; }
    }
}
