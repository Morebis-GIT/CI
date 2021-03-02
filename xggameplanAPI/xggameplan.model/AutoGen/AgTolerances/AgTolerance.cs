using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgTolerance
    {
        [XmlElement(ElementName = "atol_data.aper_no")]
        public int ScenarioNo { get; set; }

        [XmlElement(ElementName = "atol_data.atol_type")]
        public int ToleranceNo { get; set; }

        [XmlElement(ElementName = "atol_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "atol_data.min_percent")]
        public int MinimumPercent { get; set; }

        [XmlElement(ElementName = "atol_data.max_percent")]
        public int MaxPercent { get; set; }

        [XmlElement(ElementName = "ignore_yn")]
        public string IgnoreYn { get; set; }

        [XmlElement(ElementName = "book_until_max_yn")]
        public string BookUntilMaxYn { get; set; }

        [XmlElement(ElementName = "book_until_min_yn")]
        public string BookUntilMinYn { get; set; }

        [XmlElement(ElementName = "book_target_area_req_yn")]
        public string BookTargetAreaYn { get; set; }
    }
}
