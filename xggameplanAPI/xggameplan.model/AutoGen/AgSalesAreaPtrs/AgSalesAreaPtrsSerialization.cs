using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSalesAreaPtrsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSalesAreaPtr> AgSalesAreaPtrs { get; set; }

        /// <summary>
        /// populate dynamic ag SalesArea Ptr Serialization
        /// </summary>
        /// <param name="agSalesAreaPtrs"></param>
        /// <returns></returns>
        public AgSalesAreaPtrsSerialization MapFrom(List<AgSalesAreaPtr> agSalesAreaPtrs)
        {
            AgSalesAreaPtrs = agSalesAreaPtrs;
            Size = agSalesAreaPtrs.Count;

            return this;
        }
    }
}
