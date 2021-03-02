using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgCampaigns
{
    public class AgCampaignInclusion
    {
        [XmlElement(ElementName = "camp.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "camp.incl_functions")]
        public int IncludeFunctions { get; set; }
    }
}
