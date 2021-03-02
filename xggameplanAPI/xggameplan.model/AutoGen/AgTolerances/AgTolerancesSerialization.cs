using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgTolerancesSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgTolerance> AgTolerances { get; set; }

        /// <summary>
        /// Dynamic ag tolerance xml population
        /// </summary>
        /// <param name="agTolerances"></param>
        /// <returns></returns>
        public AgTolerancesSerialization MapFrom(List<AgTolerance> agTolerances)
        {
            AgTolerances = agTolerances;
            Size = agTolerances.Count;
            return this;
        }
    }
}
