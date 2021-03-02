using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSchoolHolidaysSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgSchoolHolidays list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSchoolHoliday> AgSchoolHolidays { get; set; }

        /// <summary>
        /// Dynamic ag public holiday xml population
        /// </summary>
        /// <param name="agSchoolHolidays"></param>
        /// <returns></returns>
        public AgSchoolHolidaysSerialization MapFrom(List<AgSchoolHoliday> agSchoolHolidays)
        {
            AgSchoolHolidays = agSchoolHolidays;
            Size = agSchoolHolidays.Count;
            return this;
        }
    }
}
