using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgPrediction
    {
        [XmlElement(ElementName = "pred.demo_no")]
        public int DemographicNo { get; set; }

        [XmlElement(ElementName = "pred.pred_sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "pred.no_of_rtgs")]
        public double NoOfRtgs { get; set; }
    }
}
