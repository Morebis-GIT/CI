using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgExposure
    {
        [XmlElement(ElementName = "noex.v_brek_sare_no")]
        public int BreakSalesAreaNo { get; set; }

        [XmlElement(ElementName = "v_clsh_code")]
        public string ClashCode { get; set; }

        [XmlElement(ElementName = "v_mast_clsh_code")]
        public string MasterClashCode { get; set; }

        [XmlElement(ElementName = "noex.v_start_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "noex.v_end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "noex.v_start_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "noex.v_end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "noex.v_start_day")]
        public int StartDay { get; set; }

        [XmlElement(ElementName = "noex.v_end_day")]
        public int EndDay { get; set; }

        [XmlElement(ElementName = "noex.v_no_of_exposures")]
        public int NoOfExposures { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgExposure"/></summary>
        public AgExposure Clone() => (AgExposure)MemberwiseClone();
    }
}
