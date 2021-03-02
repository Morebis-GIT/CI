using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSalesAreaDemographicsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSalesAreaDemographic> AgSalesAreaDemographics { get; set; }

        /// <summary>
        /// Populate dynamic AgSalesAreaDemographics Serialization
        /// </summary>
        /// <param name="agSalesAreaDemographics"></param>
        /// <returns></returns>
        public static AgSalesAreaDemographicsSerialization MapFrom(List<AgSalesAreaDemographic> agSalesAreaDemographics)
        {
            return new AgSalesAreaDemographicsSerialization
            {
                AgSalesAreaDemographics = agSalesAreaDemographics,
                Size = agSalesAreaDemographics?.Count ?? 0
            };
        }
    }
}
