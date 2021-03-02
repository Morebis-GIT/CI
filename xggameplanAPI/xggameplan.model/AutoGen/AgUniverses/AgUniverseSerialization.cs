using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgUniversesSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgUniverse> AgUniverses { get; set; }

        /// <summary>
        /// Dynamic ag universe xml population
        /// </summary>
        /// <param name="agUniverses"></param>
        /// <returns></returns>
        public AgUniversesSerialization MapFrom(List<AgUniverse> agUniverses)
        {
            AgUniverses = agUniverses;
            Size = agUniverses.Count;
            return this;
        }
    }
}
