using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgStandardDayParts
{
    [XmlRoot(ElementName = "Item")]
    public class AgStandardDayPartTimeslice
    {
        [XmlElement(ElementName = "sdpt_list.stt_day")]
        public int StartDay { get; set; }

        [XmlElement(ElementName = "sdpt_list.end_day")]
        public int EndDay { get; set; }

        [XmlElement(ElementName = "sdpt_list.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "sdpt_list.end_time")]
        public string EndTime { get; set; }
    }
}
