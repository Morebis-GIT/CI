using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgParam
    {
        [XmlElement(ElementName = "param_data.aper_no")]
        public int ScenarioNumber { get; set; }

        [XmlElement(ElementName = "param_data.abpn_no")]
        public int DupScenarioNumber { get; set; }

        [XmlElement(ElementName = "param_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "param_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "param_data.system_date")]
        public string SystemDate { get; set; }

        [XmlElement(ElementName = "param_data.spread_programming")]
        public int SpreadProgramming { get; set; }

        [XmlElement(ElementName = "param_data.efficiency_period")]
        public int EfficiencyPeriod { get; set; }

        [XmlElement(ElementName = "param_data.effe_no_of_weeks")]
        public int NumberOfWeeks { get; set; }

        [XmlElement(ElementName = "param_data.ignore_zero_perc_sare")]
        public int IgnoreZeroPercentageSplit { get; set; }

        [XmlElement(ElementName = "param_data.book_target_area_req")]
        public int BookTargetArea { get; set; }

        [XmlElement(ElementName = "param_data.zero_rating_value")]
        public double ZeroRatingValue { get; set; }

        [XmlElement(ElementName = "param_data.skip_locked_breaks")]
        public int SkipLockedBreaks { get; set; }

        [XmlElement(ElementName = "param_data.ignore_premium_breaks")]
        public int IgnorePremiumCategoryBreaks { get; set; }

        [XmlElement(ElementName = "param_data.excl_bank_hols")]
        public int ExcludeBankHolidays { get; set; }

        [XmlElement(ElementName = "param_data.excl_school_hols")]
        public int ExcludeSchoolHolidays { get; set; }

        [XmlElement(ElementName = "param_data.unsold_factor")]
        public double OpenAirtimeFactor { get; set; }

        [XmlElement(ElementName = "posn_in_prog")]
        public string PositionInProgramme { get; set; }
    }
}
