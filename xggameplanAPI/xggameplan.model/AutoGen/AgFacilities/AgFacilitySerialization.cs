using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgFacilitySerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgFacility> AgFacilities { get; set; }

        /// <summary>
        /// Populate dynamic AgFacility Serialization
        /// </summary>
        /// <param name="agFacilities"></param>
        /// <returns></returns>
        public static AgFacilitySerialization MapFrom(List<AgFacility> agFacilities)
        {
            return new AgFacilitySerialization
            {
                AgFacilities = agFacilities,
                Size = agFacilities?.Count ?? 0
            };
        }
    }
}
