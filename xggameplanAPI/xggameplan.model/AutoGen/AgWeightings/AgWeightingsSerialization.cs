using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgWeightingsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgWeighting> AgWeightings { get; set; }

        /// <summary>
        /// Dynamic ag weighting xml population
        /// </summary>
        /// <param name="agWeightings"></param>
        /// <returns></returns>
        public AgWeightingsSerialization MapFrom(List<AgWeighting> agWeightings)
        {
            AgWeightings = agWeightings;
            Size = agWeightings.Count;
            return this;
        }
    }
}
