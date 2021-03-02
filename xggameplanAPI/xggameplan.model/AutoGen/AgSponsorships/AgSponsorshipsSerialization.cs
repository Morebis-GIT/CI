using System.Collections.Generic;
using System.Xml.Serialization;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSponsorshipsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSponsorship> AgSponsorships { get; set; }

        /// <summary>
        /// Dynamic ag public holiday xml population
        /// </summary>
        /// <param name="agSchoolHolidays"></param>
        /// <returns></returns>
        public AgSponsorshipsSerialization MapFrom(List<AgSponsorship> agSponsorships)
        {
            AgSponsorships = agSponsorships;
            Size = agSponsorships.Count;
            return this;
        }
    }
}
