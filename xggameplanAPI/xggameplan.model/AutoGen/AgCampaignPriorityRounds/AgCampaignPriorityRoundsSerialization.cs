using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgCampaignPriorityRoundsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgCampaignPriorityRounds list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgCampaignPriorityRound> AgCampaignPriorityRounds { get; set; }

        /// <summary>
        /// Dynamic ag campaign priority round xml population
        /// </summary>
        /// <param name="agCampaignPriorityRounds"></param>
        /// <returns></returns>
        public AgCampaignPriorityRoundsSerialization MapFrom(List<AgCampaignPriorityRound> agCampaignPriorityRounds)
        {
            AgCampaignPriorityRounds = agCampaignPriorityRounds;
            Size = agCampaignPriorityRounds.Count;
            return this;
        }
    }
}
