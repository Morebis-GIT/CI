using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.model.AutoGen.AgCampaignBreakRequirements
{
    public class AgCampaignBreakRequirement
    {
        [XmlElement(ElementName = "cend_data.camp_no")]
        public int CampaignId { get; set; }

        [XmlElement(ElementName = "cend_data.sare_no")]
        public int SalesAreaId { get; set; }

        [XmlElement(ElementName = "cend_data.centre_break_reqm")]
        public AgRequirement CentreBreakRequirement { get; set; }

        [XmlElement(ElementName = "cend_data.end_break_reqm")]
        public AgRequirement EndBreakRequirement { get; set; }
    }
}
