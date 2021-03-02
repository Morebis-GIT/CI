using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgTotalRating
    {
        [XmlElement(ElementName = "trdp_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "trdp_data.demo_no")]
        public int DemographNo { get; set; }

        [XmlElement(ElementName = "trdp_data.sdpg_no")]
        public int DaypartGroupNo { get; set; }

        [XmlElement(ElementName = "trdp_data.stdp_no")]
        public int DaypartNo { get; set; }

        [XmlElement(ElementName = "trdp_data.trdp_date")]
        public string Date { get; set; }

        [XmlElement(ElementName = "trdp_data.tot_rtgs")]
        public double TotalRatings { get; set; }
    }
}
