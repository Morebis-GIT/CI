using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgClashExceptionsSerialisation : BoostSerialization
    {
        //Size - needs to be populated based on the AgClashExceptions list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgClashException> AgClashExceptions { get; set; }

        public AgClashExceptionsSerialisation MapFrom(List<AgClashException> agClashExclusions)
        {
            AgClashExceptions = agClashExclusions;
            Size = agClashExclusions.Count;
            return this;
        }
    }
}
