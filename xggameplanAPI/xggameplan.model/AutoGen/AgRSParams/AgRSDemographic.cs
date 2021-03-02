using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "Item")]
    public class AgRSDemographic
    {
        [XmlElement(ElementName = "rs_demo_data.demo_no")]
        public int DemographicNo { get; set; }

        [XmlElement(ElementName = "rs_demo_data.nbr_rules")]
        public int NbrDeliverySettings { get; set; }

        [XmlArray(ElementName = "rule_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgRSDeliverySettingsList AgRSDeliverySettingsList { get; set; }
    }

    public class AgRSDemographics : List<AgRSDemographic> { }
}
