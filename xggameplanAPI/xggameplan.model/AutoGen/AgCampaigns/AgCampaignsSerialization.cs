using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgCampaignsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgCampaign> AgCampaigns { get; set; }

        public AgCampaignsSerialization MapFrom(List<AgCampaign> agCampaigns)
        {
            AgCampaigns = agCampaigns;
            Size = agCampaigns?.Count ?? 0;
            return this;
        }
    }
}
