using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgDefaultsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgDefault> AgDefaults { get; set; }

        /// <summary>
        /// Dynamic ag default xml population
        /// </summary>
        /// <param name="agDefaults"></param>
        /// <returns></returns>
        public AgDefaultsSerialization MapFrom(List<AgDefault> agDefaults)
        {
            AgDefaults = agDefaults;
            Size = agDefaults.Count;
            return this;
        }
    }
}
