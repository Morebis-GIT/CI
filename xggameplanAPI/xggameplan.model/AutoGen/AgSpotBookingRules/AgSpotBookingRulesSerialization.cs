using System.Collections.Generic;
using System.Xml.Serialization;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgSpotBookingRules
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgSpotBookingRulesSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgSpotBookingRule> AgSpotBookingRules { get; set; }

        /// <summary>
        /// Populate dynamic AgSpotBookingRules Serialization
        /// </summary>
        /// <param name="agSpotBookingRules"></param>
        /// <returns></returns>
        public AgSpotBookingRulesSerialization MapFrom(List<AgSpotBookingRule> agSpotBookingRules)
        {
            return new AgSpotBookingRulesSerialization
            {
                AgSpotBookingRules = agSpotBookingRules,
                Size = agSpotBookingRules?.Count ?? 0
            };
        }
    }
}
