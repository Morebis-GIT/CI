using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgPublicHoliday
    {
        [XmlElement(ElementName = "phol_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "phol_data.date_date")]
        public string Date { get; set; }
    }
}
