using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgSlottingLimit
    {
        [XmlElement(ElementName = "abdm_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "abdm_data.demo_no")]
        public int DemographNo { get; set; }

        [XmlElement(ElementName = "abdm_data.max_eff_rank")]
        public int MaximumEfficiency { get; set; }

        [XmlElement(ElementName = "abdm_data.min_efficiency")]
        public int MinimumEfficiency { get; set; }

        [XmlElement(ElementName = "abdm_data.demo_band_tolerance")]
        public int BandingTolerance { get; set; }
    }
}
