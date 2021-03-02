using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "Item")]
    public class AgRSParam
    {
        [XmlElement(ElementName = "rs_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "rs_data.excl_prog_reqs_spots")]
        public int ExcludeProgrammeSpots { get; set; }

        [XmlElement(ElementName = "rs_data.nbr_rules")]
        public int NbrRules { get; set; }

        [XmlArray(ElementName = "rule_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgRSDeliverySettingsList AgRSDeliverySettingsList { get; set; }

        [XmlElement(ElementName = "rs_data.nbr_demos")]
        public int NbrDemographics { get; set; }

        [XmlArray(ElementName = "demo_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgRSDemographics AgRSDemographics { get; set; }
    }
}
