using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSalesAreaRefsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSalesAreaRef> AgSalesAreaRefs { get; set; }

        public AgSalesAreaRefsSerialization MapFrom(List<AgSalesAreaRef> agSalesAreaRefs)
        {
            AgSalesAreaRefs = agSalesAreaRefs;
            Size = agSalesAreaRefs.Count;
            return this;
        }
    }
}
