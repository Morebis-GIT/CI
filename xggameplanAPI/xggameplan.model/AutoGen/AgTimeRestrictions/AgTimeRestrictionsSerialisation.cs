using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgTimeRestrictionsSerialisation : BoostSerialization
    {
        //Size - needs to be populated based on the AgTimeRestrictions list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgTimeRestriction> AgTimeRestrictions { get; set; }

        public AgTimeRestrictionsSerialisation MapFrom(List<AgTimeRestriction> agTimeRestrictions)
        {
            AgTimeRestrictions = agTimeRestrictions;
            Size = agTimeRestrictions?.Count ?? 0;
            return this;
        }
    }
}
