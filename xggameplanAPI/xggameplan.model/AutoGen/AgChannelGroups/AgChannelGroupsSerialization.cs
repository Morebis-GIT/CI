using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgChannelGroupsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgChannelGroup list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgChannelGroup> AgChannelGroups { get; set; }

        public AgChannelGroupsSerialization MapFrom(List<AgChannelGroup> agChannelGroups)
        {
            AgChannelGroups = agChannelGroups;
            Size = agChannelGroups?.Count ?? 0;
            return this;
        }
    }
}
