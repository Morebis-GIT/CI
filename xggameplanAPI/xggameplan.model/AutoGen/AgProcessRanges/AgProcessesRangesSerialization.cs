using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgProcessesRangesSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgProcessRange> AgProcessRanges { get; set; }

        public static AgProcessesRangesSerialization Create(List<AgProcessRange> agProcessRanges)
        {
            return new AgProcessesRangesSerialization()
            {
                AgProcessRanges = agProcessRanges,
                Size = agProcessRanges.Count
            };
        }
    }
}
