using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgCampaigns;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgCampaignInclusionList
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgCampaignInclusionListSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgCampaignInclusion> AgCampaignInclusions { get; set; }

        /// <summary>
        /// Populate dynamic AgCampaignInclusion Serialization
        /// </summary>
        /// <param name="agCampaignInclusions"></param>
        /// <returns></returns>
        public AgCampaignInclusionListSerialization MapFrom(List<AgCampaignInclusion> agCampaignInclusions)
        {
            AgCampaignInclusions = agCampaignInclusions;
            Size = agCampaignInclusions?.Count ?? 0;
            return this;
        }
    }
}
