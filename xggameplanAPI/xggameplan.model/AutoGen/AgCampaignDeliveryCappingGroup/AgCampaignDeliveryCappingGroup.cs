using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgCampaignDeliveryCappingGroup
    {
        [XmlElement(ElementName = "camp_abdg_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "camp_abdg_data.abdg_no")]
        public int DeliveryCappingGroupId { get; set; }
    }
}
