using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgISRParamsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgParams list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgISRParam> AgISRParams { get; set; }

        /// <summary>
        /// Dynamic ag params xml population
        /// </summary>
        /// <param name="agIRSParams"></param>
        /// <returns></returns>
        public AgISRParamsSerialization MapFrom(List<AgISRParam> agIRSParams)
        {
            AgISRParams = agIRSParams;
            Size = agIRSParams.Count;
            return this;
        }
    }
}
