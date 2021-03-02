using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgRestriction
    {
        /// <summary>
        /// Product Code for which the restriction applies, if entered, clsh_code & cbac_code must be blank
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.prod_code")]
        public int ProductCode { get; set; }

        /// <summary>
        /// Clash Code for which the restriction applies, if entered, prod_code, ccpy_code & cbac_code must be blank/zero
        /// </summary>
        [XmlElement(ElementName = "clsh_code")]
        public string ClashCode { get; set; }

        /// <summary>
        /// Copy Code for which the restriction applies, can only be entered if prod_code is populated,
        /// if entered, clsh_code & cbac_code must be blank
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.ccpy_code")]
        public string CopyCode { get; set; }

        /// <summary>
        /// Clearance Code for which the restriction applies, if entered, prod_code, ccpy_code & clsh_code must be blank/zero
        /// </summary>
        [XmlElement(ElementName = "cbac_code")]
        public string ClearanceCode { get; set; }

        /// <summary>
        /// Sales Area Number for the Restriction
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.sare_no")]
        public int SalesAreaNo { get; set; }

        /// <summary>
        /// Programme Category, if entered prog_no, epis_no, prog_class_code, index_type and index_threshold must be blank/zero
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.prgc_no")]
        public int ProgCategoryNo { get; set; }

        /// <summary>
        /// Programme, if entered prgc_no, prog_class_code, index_type and index_threshold must be blank/zero
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.prog_no")]
        public int ProgrammeNo { get; set; }

        /// <summary>
        /// Episode, can only be entered if prog_no is populated, if entered prgc_no, prog_class_code,
        /// index_type and index_threshold must be blank/zero. For nine, its always zero.
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.epis_no")]
        public int EpisodeNo { get; set; }

        /// <summary>
        /// Start Date of the Restriction YYYYMMDD
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.rsmt_effe_stt_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// End Date of the Restriction YYYYMMDD
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.effe_end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// Index Type
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.index_type")]
        public int IndexType { get; set; }

        /// <summary>
        /// Index Threshold
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.index_threshold")]
        public int IndexThreshold { get; set; }

        /// <summary>
        ///  Public Holiday Indicator; Y=Only Public Holidays; N=Not Public Holidays; X=Either
        /// </summary>
        [XmlElement(ElementName = "rsmt_phol_ind")]
        public string PublicHolidayIndicator { get; set; }

        /// <summary>
        /// School Holiday Indicator; Y=Only School Holidays; N=Not School Holidays; X=Either
        /// </summary>
        [XmlElement(ElementName = "rsmt_shol_ind")]
        public string SchoolHolidayIndicator { get; set; }

        /// <summary>
        /// Type of Restriction; 1=Time; 2=Programme/Episode; 3=Programme Category; 4=Index; 5=Programme Classification
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.rest_type")]
        public int RestrictionType { get; set; }

        /// <summary>
        /// Days; Sum of Numeric days of week; 1=Mon; 2=Tue; 4=Wed; 8=Thu; 16=Fri; 32=Sat; 64=Sun. So 127 = Mon-Sun; 5 = Mon + Wed
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.rest_days")]
        public int RestrictionDays { get; set; }

        /// <summary>
        /// Start Time of the Restriction HHMMSS
        /// defaulted to 0 if not included.
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.rest_stt_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// End Time of the Restriction HHMMSS
        /// defaulted to 995959 if not included.
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// Tolerance Time, before start of Programme, in Minutes
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.tolerance_mins_before")]
        public int TimeToleranceMinsBefore { get; set; }

        /// <summary>
        /// Tolerance Time, after end of Programme, in Minutes
        /// </summary>
        [XmlElement(ElementName = "rest_detail_data.tolerance_mins_after")]
        public int TimeToleranceMinsAfter { get; set; }

        /// <summary>
        /// Programme Classification Code
        /// </summary>
        [XmlElement(ElementName = "prog_class_code")]
        public string ProgClassCode { get; set; }

        /// <summary>
        /// Programme Classification Include (I) / Exclude (E)
        /// </summary>
        [XmlElement(ElementName = "prog_class_incl_excl")]
        public string ProgClassFlag { get; set; }

        /// <summary>
        /// Live Broadcast Include (I) / Exclude (E)
        /// </summary>
        [XmlElement(ElementName = "live_broadcast_incl_excl")]
        public string LiveBroadcastFlag { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgRestriction"/></summary>
        public AgRestriction Clone() => (AgRestriction)MemberwiseClone();
    }
}
