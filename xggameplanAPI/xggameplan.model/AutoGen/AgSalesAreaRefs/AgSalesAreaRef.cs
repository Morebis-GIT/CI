using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "item")]
    public class AgSalesAreaRef
    {
        [XmlElement(ElementName = "sare_xref.detl_sare_no")]
        public int SalesAreaNo { get; set; }
    }
}
