using System.Collections.Generic;
using System.Xml.Serialization;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgStandardDayPartGroups
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgStandardDayPartGroupsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgStandardDayPartGroup> AgStandardDayPartGroups { get; set; }

        /// <summary>
        /// Populate dynamic AgStandardDayPartGroups Serialization
        /// </summary>
        /// <param name="agStandardDayPartGroups"></param>
        /// <returns></returns>
        public AgStandardDayPartGroupsSerialization MapFrom(List<AgStandardDayPartGroup> agStandardDayPartGroups)
        {
            return new AgStandardDayPartGroupsSerialization
            {
                AgStandardDayPartGroups = agStandardDayPartGroups,
                Size = agStandardDayPartGroups?.Count ?? 0
            };
        }
    }
}
