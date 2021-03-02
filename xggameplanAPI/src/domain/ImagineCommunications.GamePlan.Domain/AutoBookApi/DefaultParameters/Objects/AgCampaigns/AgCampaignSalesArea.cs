using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgCampaignSalesArea
    {
        [XmlElement(ElementName = "saoc_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "saoc_data.tgt_sare_no")]
        public int ChannelGroupNo { get; set; }

        [XmlElement(ElementName = "saoc_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "saoc_data.revenue_perc")]
        public double RevenuePercentage { get; set; }

        [XmlElement(ElementName = "saoc_data.multipart_only")]
        public int MultiPartOnly { get; set; }

        [XmlElement(ElementName = "stop_booking")]
        public string StopBooking { get; set; }

        [XmlElement(ElementName = "saoc_data.sare_ptr")]
        public AgCampaignSalesAreaPtrRef AgCampaignSalesAreaPtrRef { get; set; }

        [XmlElement(ElementName = "saoc_data.reqm")]
        public AgRequirement AgSalesAreaCampaignRequirement { get; set; }

        [XmlElement(ElementName = "saoc_data.nbr_dlens")]
        public int NbrAgLengths { get; set; }

        [XmlArray("dlen_list")]
        [XmlArrayItem("item")]
        public AgLengths AgLengths { get; set; }

        [XmlElement(ElementName = "saoc_data.max_breks")]
        public int MaxBreaks { get; set; }

        [XmlElement(ElementName = "saoc_data.nbr_ddpds")]
        public int NbrAgStrikeWeights { get; set; }

        [XmlArray("ddpd_list")]
        [XmlArrayItem("item")]
        public AgStrikeWeights AgStrikeWeights { get; set; }

        [XmlElement(ElementName = "saoc_data.nbr_dcdps")]
        public int NbrAgDayParts { get; set; }

        [XmlArray("dcdp_list")]
        [XmlArrayItem("item")]
        public AgDayParts AgDayParts { get; set; }

        [XmlElement(ElementName = "saoc_data.nbr_parts")]
        public int NbrParts { get; set; }

        [XmlArray("part_list")]
        [XmlArrayItem("item")]
        public AgParts AgParts { get; set; }

        [XmlElement(ElementName = "saoc_data.nbr_part_dlens")]
        public int NbrPartsLengths { get; set; }

        [XmlArray("part_dlen_list")]
        [XmlArrayItem("item")]
        public AgPartLengths AgPartsLengths { get; set; }

        [XmlElement(ElementName = "saoc_data.centre_break_ratio")]
        public int CentreBreakRatio { get; set; }

        [XmlElement(ElementName = "saoc_data.end_break_ratio")]
        public int EndBreakRatio { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgCampaignSalesArea"/></summary>
        public AgCampaignSalesArea Clone() => (AgCampaignSalesArea)MemberwiseClone();
    }
}
