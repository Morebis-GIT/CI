using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSalesAreaPassPrioritySerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSalesAreaPassPriority> AgSalesAreaPassPriorities { get; set; }

        /// <summary>
        /// Dynamic ag pass default xml population
        /// </summary>
        /// <param name="agPassDefaults"></param>
        /// <returns></returns>
        public AgSalesAreaPassPrioritySerialization MapFrom(List<AgSalesAreaPassPriority> agSalesAreaPassPriorities)
        {
            AgSalesAreaPassPriorities = agSalesAreaPassPriorities;
            Size = agSalesAreaPassPriorities.Count;
            return this;
        }
    }
}
