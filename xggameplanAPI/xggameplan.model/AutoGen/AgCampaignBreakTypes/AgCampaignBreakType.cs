using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgCampaignBreakType
    {

        [XmlElement(ElementName = "cmbt_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "btyp_code")]
        public string BreakType { get; set; }

    }
}
