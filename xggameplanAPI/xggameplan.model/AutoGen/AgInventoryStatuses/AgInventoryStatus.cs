using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgInventoryStatus
    {
        [XmlElement(ElementName = "invh_code")]
        public string InventoryCode { get; set; }
    }
}
