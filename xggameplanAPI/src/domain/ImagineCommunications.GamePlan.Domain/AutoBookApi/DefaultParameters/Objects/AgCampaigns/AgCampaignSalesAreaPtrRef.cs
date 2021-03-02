using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "saoc_data.sare_ptr")]
    public class AgCampaignSalesAreaPtrRef
    {
        [XmlElement(ElementName = "sare.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlAttribute(AttributeName = "class_id")]
        public int ClassId { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgCampaignSalesAreaPtrRef"/></summary>
        public AgCampaignSalesAreaPtrRef Clone() => (AgCampaignSalesAreaPtrRef)MemberwiseClone();
    }
}
