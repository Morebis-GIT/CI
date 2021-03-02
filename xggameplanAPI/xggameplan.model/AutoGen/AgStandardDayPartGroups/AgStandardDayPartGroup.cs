using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgStandardDayPartGroups
{
    public class AgStandardDayPartGroup
    {
        [XmlElement(ElementName = "sdpg_data.sdpg_no")]
        public int DayPartGroupNo { get; set; }

        [XmlElement(ElementName = "sdpg_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "sdpg_data.demo_no")]
        public int DemographicNo { get; set; }

        [XmlElement(ElementName = "optimizer")]
        public string Optimizer { get; set; }

        [XmlElement(ElementName = "policy_group")]
        public string Policy { get; set; }

        [XmlElement(ElementName = "rtg_replacement")]
        public string RatingReplacement { get; set; }

        [XmlElement(ElementName = "sdpg_data.nbr_stdps")]
        public int NbrDayParts { get; set; }

        [XmlArray("stdp_list")]
        [XmlArrayItem("item")]
        public List<AgStandardDayPartSplit> Splits { get; set; }
    }
}
