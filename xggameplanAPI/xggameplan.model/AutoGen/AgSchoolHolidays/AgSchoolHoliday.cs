using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgSchoolHoliday
    {
        [XmlElement(ElementName = "shol_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "shol_data.date_date")]
        public string Date { get; set; }
    }
}
