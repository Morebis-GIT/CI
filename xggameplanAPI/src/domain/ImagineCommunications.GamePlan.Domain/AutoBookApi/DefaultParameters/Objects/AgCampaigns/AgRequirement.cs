using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "Item")]
    public class AgRequirement
    {
        [XmlElement(ElementName = "reqm.required")]
        public decimal Required { get; set; }

        [XmlElement(ElementName = "reqm.tgt_required")]
        public decimal TgtRequired { get; set; }

        [XmlElement(ElementName = "reqm.sare_required")]
        public decimal SareRequired { get; set; }

        [XmlElement(ElementName = "reqm.supplied")]
        public decimal Supplied { get; set; }
    }
}
