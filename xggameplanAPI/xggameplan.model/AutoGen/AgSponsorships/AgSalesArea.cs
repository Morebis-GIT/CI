using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    public class AgSalesArea
    {
        [XmlElement(ElementName = "sare_list.sare_no")]
        public int SalesArea { get; set; }
    }
}
