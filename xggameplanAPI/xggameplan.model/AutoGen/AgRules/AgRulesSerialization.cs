using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgRulesSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgRule> AgRules { get; set; }

        /// <summary>
        /// Dynamic ag rule xml population
        /// </summary>
        /// <param name="agRules"></param>
        /// <returns></returns>
        public AgRulesSerialization MapFrom(List<AgRule> agRules)
        {
            AgRules = agRules;
            Size = agRules.Count;
            return this;
        }
    }
}
