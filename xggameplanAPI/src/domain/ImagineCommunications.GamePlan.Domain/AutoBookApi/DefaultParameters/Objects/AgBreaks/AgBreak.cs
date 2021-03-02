using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "Item")]
    public class AgBreak
    {
        [XmlElement(ElementName = "brek.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "brek.sare_id")]
        public int SalesAreaId { get; set; }

        [XmlElement(ElementName = "brek.brek_sched_date")]
        public string ScheduledDate { get; set; }

        [XmlElement(ElementName = "brek.break_no")]
        public int BreakNo { get; set; }

        [XmlElement(ElementName = "brek.broadcast_date")]
        public string BroadcastDate { get; set; }

        [XmlElement(ElementName = "brek.broadcast_clock_hour")]
        public int ClockHour { get; set; }

        [XmlElement(ElementName = "ext_no")]
        public string ExternalNo { get; set; }

        [XmlElement(ElementName = "brek.brek_nom_time")]
        public string NominalTime { get; set; }

        [XmlElement(ElementName = "brek.brek_uniq_id")]
        public int Uid { get; set; }

        [XmlElement(ElementName = "brek.prgt_no")]
        public int ProgEventNo { get; set; }

        [XmlElement(ElementName = "brek.duration")]
        public int Duration { get; set; }

        [XmlElement(ElementName = "solus")]
        public string Solus { get; set; }

        [XmlElement(ElementName = "brek.day_number")]
        public int DayNumber { get; set; }

        [XmlElement(ElementName = "posn_in_prog")]
        public string PositionInProg { get; set; }

        [XmlElement(ElementName = "brek.prog_no")]
        public int ProgNo { get; set; }

        [XmlElement(ElementName = "brek.epis_no")]
        public int EpisNo { get; set; }

        [XmlElement(ElementName = "btyp_code")]
        public string BreakTypeCode { get; set; }

        [XmlElement(ElementName = "bcat_code")]
        public string PremiumCategory { get; set; }

        [XmlElement(ElementName = "allow_split_yn")]
        public string AllowSplit { get; set; }

        [XmlElement(ElementName = "nat_reg_split_yn")]
        public string NationalRegionalSplit { get; set; }

        [XmlElement(ElementName = "exclude_packages_yn")]
        public string ExcludePackages { get; set; }

        [XmlElement(ElementName = "bonus_allowed_yn")]
        public string BonusAllowed { get; set; }

        [XmlElement(ElementName = "brek.nbr_bkrgs")]
        public int NbrBkrgs { get; set; }

        [XmlArray("bkrg_list")]
        [XmlArrayItem("item")]
        public AgRegionalBreaks AgRegionalBreaks { get; set; }

        [XmlElement(ElementName = "brek.nbr_zero_bkrgs")]
        public int NbrZeroBkrgs { get; set; }

        [XmlElement(ElementName = "brek.nbr_preds")]
        public int NbrPreds { get; set; }

        [XmlArray("pred_list")]
        [XmlArrayItem("item")]
        public AgPredictions AgPredictions { get; set; }

        [XmlElement(ElementName = "brek.nbr_avals")]
        public int NbrAvals { get; set; }

        [XmlArray("aval_list")]
        [XmlArrayItem("item")]
        public AgAvals AgAvals { get; set; }

        [XmlElement(ElementName = "brek.nbr_prgcs")]
        public int NbrPrgcs { get; set; }

        [XmlArray("prgc_list")]
        [XmlArrayItem("item")]
        public AgProgCategories AgProgCategories { get; set; }

        [XmlElement(ElementName = "brek.max_prgcs")]
        public int MaxPrgcs { get; set; }

        [XmlElement(ElementName = "brek.sare_ptr")]
        public AgSalesAreaPtrRef AgSalesAreaPtrRef { get; set; }

        [XmlElement(ElementName = "long_form")]
        public string LongForm { get; set; }

        [XmlElement(ElementName = "brek.break_price")]
        public double BreakPrice { get; set; }

        [XmlElement(ElementName = "brek.floor_rate")]
        public double FloorRate { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgBreak"/></summary>
        public AgBreak Clone() => (AgBreak)MemberwiseClone();
    }
}
