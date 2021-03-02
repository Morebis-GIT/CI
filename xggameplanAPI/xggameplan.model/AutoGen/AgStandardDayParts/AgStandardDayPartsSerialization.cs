using System.Collections.Generic;
using System.Xml.Serialization;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgStandardDayParts
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgStandardDayPartsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgStandardDayPart> AgStandardDayParts { get; set; }

        /// <summary>
        /// Populate dynamic AgStandardDayParts Serialization
        /// </summary>
        /// <param name="agStandardDayParts"></param>
        /// <returns></returns>
        public AgStandardDayPartsSerialization MapFrom(List<AgStandardDayPart> agStandardDayParts)
        {
            return new AgStandardDayPartsSerialization
            {
                AgStandardDayParts = agStandardDayParts,
                Size = agStandardDayParts?.Count ?? 0
            };
        }
    }
}
