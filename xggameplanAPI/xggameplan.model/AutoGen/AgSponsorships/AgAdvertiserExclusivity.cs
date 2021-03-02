using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    public class AgAdvertiserExclusivity
    {
        [XmlElement(ElementName = "advt_code")]
        public string Advertisercode { get; set; }
        [XmlElement(ElementName = "advt_excl_list.rest_type")]
        public int RestrictionType { get; set; }
        [XmlElement(ElementName = "advt_excl_list.rest_value")]
        public int RestrictionValue { get; set; }
    }
}
