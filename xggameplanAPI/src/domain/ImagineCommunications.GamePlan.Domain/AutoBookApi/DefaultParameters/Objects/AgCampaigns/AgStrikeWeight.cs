using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgStrikeWeight
    {
        [XmlElement(ElementName = "ddpd_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "ddpd_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "ddpd_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "ddpd_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "ddpd_data.nbr_cdpl")]
        public int NbrAgStrikeWeightLengths { get; set; }

        [XmlElement(ElementName = "ddpd_data.spotmax_ratings")]
        public int SpotMaxRatings { get; set; }

        [XmlArray("cdpl_list")]
        [XmlArrayItem("item")]
        public AgStrikeWeightLengths AgStrikeWeightLengths { get; set; }

        [XmlElement(ElementName = "ddpd_data.reqm")]
        public AgRequirement AgStikeWeightRequirement { get; set; }
    }
}
