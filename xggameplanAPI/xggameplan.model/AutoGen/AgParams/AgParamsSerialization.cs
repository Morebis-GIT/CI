using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgParamsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgParams list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgParam> AgParams { get; set; }

        /// <summary>
        /// Dynamic ag params xml population
        /// </summary>
        /// <param name="agParams"></param>
        /// <returns></returns>
        public AgParamsSerialization MapFrom(List<AgParam> agParams)
        {
            AgParams = agParams;
            Size = agParams.Count;
            return this;
        }
    }
}
