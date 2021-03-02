using System.Collections.Generic;
using System.Xml.Serialization;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgCampaignBreakRequirements
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgCampaignBreakRequirementsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgCampaignBreakRequirement> AgCampaignBreakRequirements { get; set; }

        /// <summary>
        /// Populate dynamic AgCampaignBreakRequirements Serialization
        /// </summary>
        /// <param name="agLengthFactors"></param>
        /// <returns></returns>
        public static AgCampaignBreakRequirementsSerialization MapFrom(List<AgCampaignBreakRequirement> agCampaignBreakRequirements)
        {
            return new AgCampaignBreakRequirementsSerialization
            {
                AgCampaignBreakRequirements = agCampaignBreakRequirements,
                Size = agCampaignBreakRequirements?.Count ?? 0
            };
        }
    }
}
