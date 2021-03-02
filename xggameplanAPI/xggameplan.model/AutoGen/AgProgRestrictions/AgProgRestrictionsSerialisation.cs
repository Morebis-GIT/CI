using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgProgRestrictionsSerialisation : BoostSerialization
    {
        //Size - needs to be populated based on the AgTimeRestrictions list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgProgRestriction> AgProgRestrictions { get; set; }

        public AgProgRestrictionsSerialisation MapFrom(List<AgProgRestriction> agProgRestrictions)
        {
            AgProgRestrictions = agProgRestrictions;
            Size = agProgRestrictions?.Count ?? 0;
            return this;
        }
    }
}
