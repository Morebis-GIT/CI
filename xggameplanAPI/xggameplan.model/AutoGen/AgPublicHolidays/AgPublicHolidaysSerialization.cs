using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgPublicHolidaysSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgBreaks list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgPublicHoliday> AgPublicHolidays { get; set; }

        /// <summary>
        /// Dynamic ag public holiday xml population
        /// </summary>
        /// <param name="agPublicHolidays"></param>
        /// <returns></returns>
        public AgPublicHolidaysSerialization MapFrom(List<AgPublicHoliday> agPublicHolidays)
        {
            AgPublicHolidays = agPublicHolidays;
            Size = agPublicHolidays.Count;
            return this;
        }
    }
}
