using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "Item")]
    public class AgRSDeliverySettings
    {
        [XmlElement(ElementName = "rs_rule_data.days")]
        public int DaysToCampaignEnd { get; set; }

        [XmlElement(ElementName = "rs_rule_data.upper")]
        public int UpperLimitOfOverDelivery { get; set; }

        [XmlElement(ElementName = "rs_rule_data.lower")]
        public int LowerLimitOfOverDelivery { get; set; }
    }

    public class AgRSDeliverySettingsList : List<AgRSDeliverySettings> { }
}
