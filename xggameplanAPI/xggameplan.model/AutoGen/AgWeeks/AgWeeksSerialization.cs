using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgWeeksSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgWeek> AgWeeks { get; set; }

        public AgWeeksSerialization MapFrom(List<AgWeek> agWeeks)
        {
            AgWeeks = agWeeks;
            Size = agWeeks.Count;
            return this;
        }
    }
}
