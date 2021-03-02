using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgStrikeWeightLength
    {
        [XmlElement(ElementName = "cdpl_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "cdpl_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "cdpl_data.length")]
        public int SpotLength { get; set; }

        [XmlElement(ElementName = "cdpl_data.multipart_no")]
        public int MultiPartNo { get; set; }

        [XmlElement(ElementName = "cdpl_data.ddpd_stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "cdpl_data.ddpd_end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "cdpl_data.reqm")]
        public AgRequirement AgStrikeWeightLengthRequirement { get; set; }
    }
}
