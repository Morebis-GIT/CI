using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "Item")]
    public class AgISRSalesArea
    {
        [XmlElement(ElementName = "isr_sare_data.sare_no")]
        public int SalesAreaNo { get; set; }
    }

    public class AgISRSalesAreas : List<AgISRSalesArea> { }
}
