using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    public class AgClashExclusivity
    {
        [XmlElement(ElementName = "clsh_code")]
        public string ClashCode { get; set; }
        [XmlElement(ElementName = "clsh_excl_list.rest_type")]
        public int RestrictionType { get; set; }
        [XmlElement(ElementName = "clsh_excl_list.rest_value")]
        public int RestrictionValue { get; set; }
    }
}
