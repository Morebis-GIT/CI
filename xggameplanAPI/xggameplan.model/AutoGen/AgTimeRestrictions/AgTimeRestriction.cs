using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgTimeRestriction
    {
        [XmlElement(ElementName = "treq_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "treq_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "treq_data.treq_day")]
        public int DayNo { get; set; }

        [XmlElement(ElementName = "treq_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "treq_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "treq_data.treq_stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "treq_data.end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "incl_excl_flag")]
        public string IncludeExcludeFlag { get; set; }
    }
}
