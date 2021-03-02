using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgFacility
    {
        [XmlElement(ElementName = "facility_code")]
        public string Code { get; set; }
    }
}
