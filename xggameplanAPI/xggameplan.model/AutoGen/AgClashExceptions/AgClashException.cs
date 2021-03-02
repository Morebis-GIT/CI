using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgClashException
    {

        [XmlElement(ElementName = "v_cexc_clsh_code")]
        public string FromClashCode { get; set; }

        [XmlElement(ElementName = "v_clsh_code")]
        public string ToClashCode { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_cexc_prod_code")]
        public int FromProductCode { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_prod_code")]
        public int ToProductCode { get; set; }

        [XmlElement(ElementName = "v_incl_excl")]
        public string IncludeExcludeFlag { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_start_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_start_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_start_day_no")]
        public int StartDayNo { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_end_day_no")]
        public int EndDayNo { get; set; }

        [XmlElement(ElementName = "clsh_exc_list.v_defined_clash")]
        public int DefinedClash { get; set; }
    }
}
