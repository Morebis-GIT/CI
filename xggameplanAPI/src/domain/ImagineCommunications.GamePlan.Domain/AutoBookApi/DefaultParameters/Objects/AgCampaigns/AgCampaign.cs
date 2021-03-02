using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "Item")]
    public class AgCampaign
    {
        [XmlElement(ElementName = "camp.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "ext_no")]
        public string ExternalNo { get; set; }

        [XmlElement(ElementName = "camp.demo_no")]
        public int DemographicNo { get; set; }

        [XmlElement(ElementName = "camp.deal_no")]
        public int DealNo { get; set; }

        [XmlElement(ElementName = "camp.prod_code")]
        public int ProductCode { get; set; }

        [XmlElement(ElementName = "cbac_code")]
        public string ClearanceCode { get; set; }

        [XmlElement(ElementName = "camp.bsar_no")]
        public int BusinesssAreaNo { get; set; }

        [XmlElement(ElementName = "camp.revenue_budget")]
        public int RevenueBudget { get; set; }

        [XmlElement(ElementName = "camp.dlvy_currency")]
        public int DeliveryCurrency { get; set; }

        [XmlElement(ElementName = "top_tail_flag")]
        public string MultiPartFlag { get; set; }

        [XmlElement(ElementName = "camp.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "camp.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "root_clsh_code")]
        public string RootClashCode { get; set; }

        [XmlElement(ElementName = "clsh_code")]
        public string ClashCode { get; set; }

        [XmlElement(ElementName = "advt_code")]
        public string AdvertiserIdentifier { get; set; }

        [XmlElement(ElementName = "camp.clsh_no")]
        public int ClashNo { get; set; }

        [XmlElement(ElementName = "stop_booking")]
        public string StopBooking { get; set; }

        [XmlElement(ElementName = "bstp_code")]
        public string BusinessType { get; set; }

        [XmlElement(ElementName = "camp.reqm")]
        public AgRequirement AgCampaignRequirement { get; set; }

        [XmlElement(ElementName = "camp.incl_functions")]
        public int IncludeFunctions { get; set; }

        [XmlElement(ElementName = "camp.nbr_saocs")]
        public int NbrAgCampagignSalesArea { get; set; }

        [XmlElement(ElementName = "camp.max_saocs")]
        public int MaxAgCampagignSalesArea { get; set; }

        [XmlElement(ElementName = "camp.spotmax_ratings")]
        public int CampaignSpotMaxRatings { get; set; }

        [XmlArray("saoc_list")]
        [XmlArrayItem("item")]
        public AgCampaignSalesAreas AgCampaignSalesAreas { get; set; }

        [XmlElement(ElementName = "camp.nbr_prog_reqs")]
        public int NbrAgCampaignProgrammeList { get; set; }

        [XmlArray("prog_reqs_list")]
        [XmlArrayItem("item")]
        public AgCampaignProgrammeList AgProgrammeList { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgCampaign"/></summary>
        public AgCampaign Clone() => (AgCampaign)MemberwiseClone();
    }
}
