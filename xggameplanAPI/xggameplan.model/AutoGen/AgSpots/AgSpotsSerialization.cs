using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSpotsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgParams list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSpot> AgSpots { get; set; }

        /// <summary>
        /// Dynamic ag params xml population
        /// </summary>
        /// <param name="agSpots"></param>
        /// <returns></returns>
        public AgSpotsSerialization MapFrom(List<AgSpot> agSpots)
        {
            AgSpots = agSpots;
            Size = agSpots.Count;
            return this;
        }
    }
}
