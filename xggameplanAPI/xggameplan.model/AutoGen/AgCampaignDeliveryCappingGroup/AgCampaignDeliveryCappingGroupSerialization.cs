using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgCampaignDeliveryCappingGroupSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgCampaignDeliveryCappingGroup> AgCampaignsDeliveryCappingGroups { get; set; }

        /// <summary>
        /// Populate dynamic ag AgCampaignDeliveryCappingGroup Serialization
        /// </summary>
        /// <param name="agCampaignDeliveryCappingGroups"></param>
        /// <returns></returns>
        public static AgCampaignDeliveryCappingGroupSerialization MapFrom(List<AgCampaignDeliveryCappingGroup> agCampaignDeliveryCappingGroups)
        {
            return new AgCampaignDeliveryCappingGroupSerialization
            {
                AgCampaignsDeliveryCappingGroups = agCampaignDeliveryCappingGroups,
                Size = agCampaignDeliveryCappingGroups?.Count ?? 0
            };
        }
    }
}
