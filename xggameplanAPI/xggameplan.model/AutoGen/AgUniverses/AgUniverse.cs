using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgUniverse
    {
        [XmlElement(ElementName = "univ_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "univ_data.demo_no")]
        public int DemographicNo { get; set; }

        [XmlElement(ElementName = "univ_data.universe")]
        public int UniverseNo { get; set; }

        [XmlElement(ElementName = "univ_data.univ_stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "univ_data.univ_end_date")]
        public string EndDate { get; set; }
    }
}
