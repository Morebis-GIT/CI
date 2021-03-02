using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "Item")]
    public class AgPartLength
    {
        [XmlElement(ElementName = "part_dlen_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "part_dlen_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "part_dlen_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "part_dlen_data.dcdp_type")]
        public int DayPartType { get; set; }

        [XmlElement(ElementName = "part_dlen_data.dcdp_no")]
        public int DayPartNo { get; set; }

        [XmlElement(ElementName = "part_dlen_data.sslg_length")]
        public long SpotLength { get; set; }

        [XmlElement(ElementName = "part_dlen_data.multipart_no")]
        public int MultipartNumber { get; set; }

        [XmlElement(ElementName = "part_dlen_data.reqm")]
        public AgRequirement AgPartLengthRequirement { get; set; }
    }
}
