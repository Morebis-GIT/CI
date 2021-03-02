using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgScenarioCampaignPassSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgScenarioCampaignPass> AgScenarioCampaignPasses { get; set; }

        /// <summary>
        /// Dynamic ag Scenario's Campaign Pass xml population
        /// </summary>
        /// <param name="agScenarioCampaignPasses"></param>
        /// <returns></returns>
        public AgScenarioCampaignPassSerialization MapFrom(List<AgScenarioCampaignPass> agScenarioCampaignPasses)
        {
            AgScenarioCampaignPasses = agScenarioCampaignPasses;
            Size = agScenarioCampaignPasses.Count;
            return this;
        }
    }
}
