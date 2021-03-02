using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgCampaignBreakTypesSerialisation : BoostSerialization
    {
        //Size - needs to be populated based on the AgCampaignBreakTypes list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgCampaignBreakType> AgCampaignBreakTypes { get; set; }

        public AgCampaignBreakTypesSerialisation MapFrom(List<AgCampaignBreakType> agCampaignBreakTypes)
        {
            AgCampaignBreakTypes = agCampaignBreakTypes;
            Size = agCampaignBreakTypes.Count;
            return this;
        }
    }
}
