using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "Item")]
    public class AgISRParam
    {
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "isr_data.start_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "isr_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "isr_data.effe_threshold")]
        public int EfficiencyThreshold { get; set; }

        [XmlElement(ElementName = "btyp_code")]
        public string BreakType { get; set; }

        [XmlElement(ElementName = "isr_data.excl_bcat")]
        public int ExcludePremiumBreaks { get; set; }

        [XmlElement(ElementName = "isr_data.excl_phol")]
        public int ExcludePublicHolidays { get; set; }

        [XmlElement(ElementName = "isr_data.excl_shol")]
        public int ExcludeSchoolHolidays { get; set; }

        [XmlElement(ElementName = "isr_data.excl_prog_reqs_spots")]
        public int ExcludeProgrammeSpots { get; set; }

        [XmlElement(ElementName = "isr_data.nbr_sares")]
        public int NbrSalesAreas { get; set; }

        [XmlArray(ElementName = "sare_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgISRSalesAreas AgISRSalesAreas { get; set; }

        [XmlElement(ElementName = "isr_data.nbr_times")]
        public int NbrTimesBands { get; set; }

        [XmlArray(ElementName = "time_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgISRTimeBands AgISRTimeBands { get; set; }

        [XmlElement(ElementName = "isr_data.nbr_demos")]
        public int NbrDemographics { get; set; }

        [XmlArray(ElementName = "demo_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgISRDemographics AgISRDemographics { get; set; }
    }
}
