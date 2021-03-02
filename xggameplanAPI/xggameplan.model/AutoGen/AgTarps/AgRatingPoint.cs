using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgRatingPoint
    {
        [XmlElement(ElementName = "min_tarp_data.abdn_no")]
        public int PassId { get; set; }
        [XmlElement(ElementName = "min_tarp_data.sare_no")]
        public int SalesAreaNumber { get; set; }
        [XmlElement(ElementName = "min_tarp_data.nbr_times")]
        public int DaypartsCount { get; set; }
        [XmlArray("time_list")]
        [XmlArrayItem("item")]
        public AgRatingPointsForDaypartsList RatingPointsForDaypartsList { get; set; }
    }
}
