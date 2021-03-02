using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgRatingPointsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgRatingPoints list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgRatingPoint> AgRatingPoints { get; set; }

        /// <summary>
        /// Dynamic ag Rating Points xml population
        /// </summary>
        /// <param name="agRatingPoints"></param>
        /// <returns></returns>
        public AgRatingPointsSerialization MapFrom(List<AgRatingPoint> agRatingPoints)
        {
            AgRatingPoints = agRatingPoints;
            Size = agRatingPoints.Count;
            return this;
        }
    }
}
