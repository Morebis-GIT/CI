using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSpotBookingRules
{
    public class AgSpotBookingRule
    {
        [XmlElement(ElementName = "sbru_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "sbru_data.min_brek_length")]
        public int MinBreakLength { get; set; }

        [XmlElement(ElementName = "sbru_data.max_brek_length")]
        public int MaxBreakLength { get; set; }

        [XmlElement(ElementName = "sbru_data.sbru_sslg_length")]
        public int SpotLength { get; set; }

        [XmlElement(ElementName = "sbru_data.max_no_spots")]
        public int MaxSpots { get; set; }

        [XmlElement(ElementName = "btyp_code")]
        public string BreakType { get; set; }
    }
}
