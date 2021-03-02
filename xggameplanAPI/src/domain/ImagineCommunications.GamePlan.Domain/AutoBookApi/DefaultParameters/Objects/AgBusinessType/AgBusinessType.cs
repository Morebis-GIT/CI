using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgBusinessType
{
    [XmlRoot(ElementName = "Item")]
    public class AgBusinessType
    {
        [XmlElement(ElementName = "bstp_code")]
        public string Code { get; set; }
    }
}
