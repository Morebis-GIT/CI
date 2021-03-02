using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgCampaignProgramme
    {
        [XmlElement(ElementName = "prog_reqs_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "prog_reqs_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "prog_reqs_data.reqm")]
        public AgRequirement AgCampaignProgrammeRequirement { get; set; }

        [XmlElement(ElementName = "prog_reqs_data.nbr_progs")]
        public int NbrCategoryOrProgrammeList { get; set; }

        [XmlArray("prog_list")]
        [XmlArrayItem("item")]
        public AgCampaignProgrammeProgrammeCategories CategoryOrProgramme { get; set; }

        [XmlElement(ElementName = "prog_reqs_data.nbr_sares")]
        public int NbrSalesAreas { get; set; }

        [XmlArray("sare_list")]
        [XmlArrayItem("item")]
        public AgSalesAreas SalesAreas { get; set; }

        [XmlElement(ElementName = "prog_reqs_data.nbr_times")]
        public int NumberTimeBands { get; set; }

        [XmlArray("time_list")]
        [XmlArrayItem("item")]
        public AgTimeBands TimeBands { get; set; }
    }
}
