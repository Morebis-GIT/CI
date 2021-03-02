using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSlottingLimitsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSlottingLimit> AgSlottingLimits { get; set; }

        /// <summary>
        /// Dynamic auto gen Slotting Limit xml population
        /// </summary>
        /// <param name="agSlottingLimits"></param>
        /// <returns></returns>
        public AgSlottingLimitsSerialization MapFrom(List<AgSlottingLimit> agSlottingLimits)
        {
            AgSlottingLimits = agSlottingLimits;
            Size = agSlottingLimits.Count;
            return this;
        }
    }
}
