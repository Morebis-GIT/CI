using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgDeliveryCappingGroupSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgDeliveryCappingGroup> AgDeliveryCappingGroups { get; set; }

        /// <summary>
        /// Populate dynamic AgDeliveryCappingGroup Serialization
        /// </summary>
        /// <param name="agDeliveryCappingGroups"></param>
        /// <returns></returns>
        public static AgDeliveryCappingGroupSerialization MapFrom(List<AgDeliveryCappingGroup> agDeliveryCappingGroups)
        {
            return new AgDeliveryCappingGroupSerialization
            {
                AgDeliveryCappingGroups = agDeliveryCappingGroups,
                Size = agDeliveryCappingGroups?.Count ?? 0
            };
        }
    }
}
