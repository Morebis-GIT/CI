using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgRestrictionsSerialisation : BoostSerialization
    {
        //Size - needs to be populated based on the AgTimeRestrictions list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgRestriction> AgRestrictions { get; set; }

        public AgRestrictionsSerialisation MapFrom(List<AgRestriction> agRestrictions)
        {
            AgRestrictions = agRestrictions;
            Size = agRestrictions.Count;
            return this;
        }
    }
}
