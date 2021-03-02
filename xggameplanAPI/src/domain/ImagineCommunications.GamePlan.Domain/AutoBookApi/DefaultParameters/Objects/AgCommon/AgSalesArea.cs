using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgSalesArea
    {
        [XmlElement(ElementName = "sare_data.sare_no")]
        public int SalesAreaNumber { get; set; }
    }
}
