using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgLength
    {
        [XmlElement(ElementName = "dlen_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "dlen_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "dlen_data.sslg_length")]
        public int SpotLength { get; set; }

        [XmlElement(ElementName = "dlen_data.multipart_no")]
        public int MultiPartNo { get; set; }

        [XmlElement(ElementName = "dlen_data.nbr_spots")]
        public int NoOfSpots { get; set; }

        [XmlElement(ElementName = "dlen_data.price_factor")]
        public double PriceFactor { get; set; }

        [XmlElement(ElementName = "dlen_data.reqm")]
        public AgRequirement AgLengthRequirement { get; set; }

        [XmlElement(ElementName = "dlen_data.nbr_mparts")]
        public int NbrAgMultiParts { get; set; }

        [XmlArray("mpart_list")]
        [XmlArrayItem("item")]
        public AgMultiParts AgMultiParts { get; set; }
    }
}
