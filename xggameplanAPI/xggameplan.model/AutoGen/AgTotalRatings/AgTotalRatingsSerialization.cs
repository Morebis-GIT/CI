using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{

    [XmlRoot(ElementName = "boost_serialization")]
    public class AgTotalRatingsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgTotalRating> AgTotalRatings { get; set; }

        /// <summary>
        /// Populate dynamic AgTotalRating Serialization
        /// </summary>
        /// <param name="agTotalRatings"></param>
        /// <returns></returns>
        public AgTotalRatingsSerialization MapFrom(List<AgTotalRating> agTotalRatings)
        {
            return new AgTotalRatingsSerialization
            {
                AgTotalRatings = agTotalRatings,
                Size = agTotalRatings?.Count ?? 0
            };
        }
    }
}
