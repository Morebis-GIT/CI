using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgBreaksSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgBreak> AgBreaks { get; set; }

        /// <summary>
        /// Dynamic ag break xml population
        /// </summary>
        /// <param name="agBreaks"></param>
        /// <returns></returns>
        public AgBreaksSerialization MapFrom(List<AgBreak> agBreaks)
        {
            AgBreaks = agBreaks;
            Size = agBreaks.Count;
            return this;
        }
    }
}
