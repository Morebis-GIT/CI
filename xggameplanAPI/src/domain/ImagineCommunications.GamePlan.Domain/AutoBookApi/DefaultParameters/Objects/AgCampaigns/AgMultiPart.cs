using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgMultiPart
    {
        [XmlElement(ElementName = "mpart_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "mpart_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "mpart_data.multipart_no")]
        public int MultiPartNo { get; set; }

        [XmlElement(ElementName = "mpart_data.sequence_no")]
        public int SeqNo { get; set; }

        [XmlElement(ElementName = "mpart_data.sslg_length")]
        public int SpotLength { get; set; }

        [XmlElement(ElementName = "mpart_data.bkpo_posn_reqm")]
        public int BookingPosition { get; set; }

        [XmlElement(ElementName = "mpart_data.price_factor")]
        public double PriceFactor { get; set; }
    }
}
