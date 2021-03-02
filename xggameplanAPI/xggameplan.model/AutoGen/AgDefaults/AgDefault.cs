using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgDefault
    {
        [XmlElement(ElementName = "agde_data.aper_no")]
        public int ScenarioNo { get; set; }

        [XmlElement(ElementName = "agde_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "agde_data.dflt_centre_ratio")]
        public int DefaultCentreBreakRatio { get; set; }

        [XmlElement(ElementName = "agde_data.max_eff_rank")]
        public int MaximumEfficiencyRank { get; set; }

        [XmlElement(ElementName = "agde_data.min_efficiency")]
        public int MinimumEfficiency { get; set; }

        [XmlElement(ElementName = "agde_data.demo_band_tolerance")]
        public int DemographicBandTolerance { get; set; }

        [XmlElement(ElementName = "agde_data.use_camp_max_spotrating")]
        public int UseCampaignMaxSpotRatings { get; set; }

        [XmlElement(ElementName = "agde_data.use_spon_excl")]
        public int UseSponsorExclusivity { get; set; }

        [XmlElement(ElementName = "agde_data.even_zero_distribution")]
        public int EvenDistributionZeroRatingSpots { get; set; }

        [XmlElement(ElementName = "agde_data.zero_rtgs_brek")]
        public int ZeroRatedBreaks { get; set; }

        [XmlElement(ElementName = "agde_data.adpg_no")]
        public int DayPartGroup { get; set; }
    }
}
