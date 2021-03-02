using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgWeek
    {
        [XmlElement(ElementName = "week_data.week_stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "week_data.week_end_date")]
        public string EndDate { get; set; }
    }

}
