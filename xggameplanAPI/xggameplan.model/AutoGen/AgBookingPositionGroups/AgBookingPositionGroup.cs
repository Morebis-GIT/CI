using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    public class AgBookingPositionGroup
    {
        public AgBookingPositionGroup()
        {
            SalesAreaNo = 0; // Unique SalesArea id, default to zero
        }

        [XmlElement(ElementName = "cmbp_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "cmbp_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "cmbp_data.bkpo_posn_reqm")]
        public int GroupId { get; set; }

        [XmlElement(ElementName = "cmbp_data.disc_schg_perc")]
        public double DiscountSurchargePercentage { get; set; }

        [XmlElement(ElementName = "cmbp_data.reqm")]
        public AgRequirement AgBookingPositionGroupRequirement { get; set; }

        [XmlElement(ElementName = "cmbp_data.nbr_bkpos")]
        public int PositionGroupAssociationsCount { get; set; }

        [XmlArray("bkpo_list")]
        [XmlArrayItem("item")]
        public AgPositionGroupAssociations PositionGroupAssociations { get; set; }
    }
}
