using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgScenarioCampaignPass
    {
        [XmlElement(ElementName = "camp_reqs_data.camp_no")]
        public int CampaignCustomNo { get; set; }

        [XmlElement(ElementName = "camp_reqs_data.priority")]
        public int Priority { get; set; }

        [XmlElement(ElementName = "camp_reqs_data.abdn_no")]
        public int PassNo { get; set; }
    }
}
