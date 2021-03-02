using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgPeakStartEndTimeSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgPeakStartEndTime> AgPeakStartEndTimes { get; set; }

        /// <summary>
        /// Peak start and end time AutoGen xml population
        /// </summary>
        /// <param name="agPeakStartEndTime"></param>
        /// <returns></returns>
        public AgPeakStartEndTimeSerialization MapFrom(List<AgPeakStartEndTime> agPeakStartEndTime)
        {
            AgPeakStartEndTimes = agPeakStartEndTime;
            Size = agPeakStartEndTime.Count;
            return this;
        }
    }
}
