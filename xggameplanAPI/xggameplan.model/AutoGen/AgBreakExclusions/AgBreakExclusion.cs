using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgBreakExclusion
    {
        [XmlElement(ElementName = "brek_excl_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "brek_excl_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "brek_excl_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "brek_excl_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "brek_excl_data.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "brek_excl_data.end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "days")]
        public string SelectableDays { get; set; }
    }
}
