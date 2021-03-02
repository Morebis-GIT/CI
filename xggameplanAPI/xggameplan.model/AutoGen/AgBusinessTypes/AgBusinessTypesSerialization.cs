using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgBusinessType;

namespace xggameplan.Model.AutoGen.AgBusinessTypes
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgBusinessTypesSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgBusinessType> AgBusinessTypes { get; set; }

        public AgBusinessTypesSerialization MapFrom(List<AgBusinessType> agBusinessTypes)
        {
            AgBusinessTypes = agBusinessTypes;
            Size = agBusinessTypes.Count;
            return this;
        }
    }
}
