using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgPassDefaultsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgPassDefault> AgPassDefaults { get; set; }

        /// <summary>
        /// Dynamic ag pass default xml population
        /// </summary>
        /// <param name="agPassDefaults"></param>
        /// <returns></returns>
        public AgPassDefaultsSerialization MapFrom(List<AgPassDefault> agPassDefaults)
        {
            AgPassDefaults = agPassDefaults;
            Size = agPassDefaults.Count;
            return this;
        }
    }
}
