using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgBreakExclusionsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgBreakExclusion> AgBreakExclusions { get; set; }

        /// <summary>
        /// Dynamic auto gen Break Exclusions xml population
        /// </summary>
        /// <param name="agBreakExclusions"></param>
        /// <returns></returns>
        public AgBreakExclusionsSerialization MapFrom(List<AgBreakExclusion> agBreakExclusions)
        {
            AgBreakExclusions = agBreakExclusions;
            Size = agBreakExclusions.Count;
            return this;
        }
    }
}
