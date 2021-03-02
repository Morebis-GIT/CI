using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgChannelGroupSalesArea
    {
        [XmlElement(ElementName = "sare_data.sare_no")]
        public int SalesAreaNo { get; set; }
    }
}
