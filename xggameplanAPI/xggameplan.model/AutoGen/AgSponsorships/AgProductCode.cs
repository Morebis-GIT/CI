using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    public class AgProductCode {
        [XmlElement(ElementName = "prod_list.prod_code")]
        public string Code { get; set; }
    }
}
