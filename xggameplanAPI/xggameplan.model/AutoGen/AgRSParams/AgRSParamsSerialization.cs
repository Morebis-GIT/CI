using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgRSParamsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgParams list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgRSParam> AgRSParams { get; set; }

        /// <summary>
        /// Dynamic ag params xml population
        /// </summary>
        /// <param name="agRSParams"></param>
        /// <returns></returns>
        public AgRSParamsSerialization MapFrom(List<AgRSParam> agRSParams)
        {
            AgRSParams = agRSParams;
            Size = agRSParams.Count;
            return this;
        }
    }
}
