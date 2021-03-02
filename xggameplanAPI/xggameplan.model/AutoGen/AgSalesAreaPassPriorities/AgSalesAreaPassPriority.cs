using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgSalesAreaPassPriority
    {
        [XmlElement(ElementName = "sare_reqs_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "sare_reqs_data.priority")]
        public int Priority { get; set; }

        [XmlElement(ElementName = "sare_reqs_data.abdn_no")]
        public int PassNo { get; set; }
    }
}
