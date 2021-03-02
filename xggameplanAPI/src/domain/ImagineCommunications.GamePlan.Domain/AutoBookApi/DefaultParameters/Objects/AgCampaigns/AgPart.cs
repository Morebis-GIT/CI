using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    //Strike weight day part list
    [XmlRoot(ElementName = "Item")]
    public class AgPart
    {
        [XmlElement(ElementName = "part_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "part_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "part_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "part_data.dcdp_no")]
        public int DayPartNo { get; set; }

        [XmlElement(ElementName = "part_data.reqm")]
        public AgRequirement AgPartRequirement { get; set; }
    }
}
