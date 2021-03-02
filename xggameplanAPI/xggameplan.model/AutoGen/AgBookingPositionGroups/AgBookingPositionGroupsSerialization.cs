using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgBookingPositionGroupsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBookingPositionGroup list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgBookingPositionGroup> AgBookingPositionGroups { get; set; }

        /// <summary>
        /// Populate dynamic AgBookingPositionGroup Serialization
        /// </summary>
        /// <param name="agBookingPositionGroups"></param>
        /// <returns></returns>
        public AgBookingPositionGroupsSerialization MapFrom(List<AgBookingPositionGroup> agBookingPositionGroups)
        {
            AgBookingPositionGroups = agBookingPositionGroups;
            Size = agBookingPositionGroups?.Count ?? 0;
            return this;
        }
    }
}
