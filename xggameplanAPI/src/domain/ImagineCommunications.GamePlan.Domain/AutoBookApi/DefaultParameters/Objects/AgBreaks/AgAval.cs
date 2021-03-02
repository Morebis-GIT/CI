using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "item")]
    public class AgAval
    {
        [XmlElement(ElementName = "aval.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "aval.open_avail")]
        public int OpenAvail { get; set; }

        [XmlElement(ElementName = "aval.reserve_duration")]
        public int ReserveDuration { get; set; }
    }
}
