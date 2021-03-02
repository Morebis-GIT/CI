using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "brek.sare_ptr")]
    public class AgSalesAreaPtrRef
    {
        [XmlElement(ElementName = "sare.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlAttribute(AttributeName = "class_id")]
        public int ClassId { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgSalesAreaPtrRef"/></summary>
        public AgSalesAreaPtrRef Clone() => (AgSalesAreaPtrRef)MemberwiseClone();
    }
}
