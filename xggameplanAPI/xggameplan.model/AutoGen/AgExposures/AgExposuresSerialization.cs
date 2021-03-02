using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgExposuresSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgExposures list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgExposure> AgExposures { get; set; }

        public AgExposuresSerialization MapFrom(List<AgExposure> agExposures)
        {
            AgExposures = agExposures;
            Size = agExposures.Count;
            return this;
        }
    }
}
