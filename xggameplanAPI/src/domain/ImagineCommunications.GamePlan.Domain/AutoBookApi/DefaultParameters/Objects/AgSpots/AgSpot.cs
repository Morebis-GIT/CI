using System.Xml.Serialization;
using static ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions.BookingPosition;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "Item")]
    public class AgSpot
    {
        [XmlElement(ElementName = "spot.sare_no")]
        public int BreakSalesAreaNo { get; set; }

        [XmlElement(ElementName = "spot.brek_sched_date")]
        public string BreakDate { get; set; }

        [XmlElement(ElementName = "spot.brek_nom_time")]
        public string BreakTime { get; set; }

        [XmlElement(ElementName = "spot.break_no")]
        public int BreakNo { get; set; }

        [XmlElement(ElementName = "spot.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "spot.spot_no")]
        public int SpotNo { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "mpart_spot_ind")]
        public string MultipartIndicator { get; set; }

        [XmlElement(ElementName = "spot.pree_tor_status")]
        public int PreempteeStatus { get; set; }

        [XmlElement(ElementName = "spot.pree_status")]
        public int PreemptorStatus { get; set; }

        [XmlElement(ElementName = "spot.bkpo_posn_reqm")]
        public int BookingPosition { get; set; } = NoDefaultPosition;

        [XmlElement(ElementName = "spot.sslg_length")]
        public int SpotLength { get; set; }

        [XmlElement(ElementName = "spot.spot_sare_no")]
        public int SpotSalesAreaNo { get; set; }

        [XmlElement(ElementName = "spot.price_factor")]
        public double PriceFactor { get; set; }

        [XmlElement(ElementName = "spot.nominal_price")]
        public decimal NominalPrice { get; set; }

        [XmlElement(ElementName = "added_value_yn")]
        public string BonusSpot { get; set; }

        [XmlElement(ElementName = "client_picked")]
        public string ClientPicked { get; set; }

        [XmlElement(ElementName = "spot.isr_locked")]
        public int ISRLocked { get; set; }

        [XmlElement(ElementName = "spot.prod_code")]
        public int ProductCode { get; set; }

        [XmlElement(ElementName = "clsh_code")]
        public string ClashCode { get; set; }

        [XmlElement(ElementName = "advt_code")]
        public string AdvertiserIdentifier { get; set; }

        [XmlElement(ElementName = "root_clsh_code")]
        public string RootClashCode { get; set; }

        [XmlElement(ElementName = "bstp_code")]
        public string CampaignBusinessType { get; set; }

        [XmlElement(ElementName = "spot.demo_no")]
        public int Demographic { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgSpot"/></summary>
        public AgSpot Clone() => (AgSpot)MemberwiseClone();
    }
}
