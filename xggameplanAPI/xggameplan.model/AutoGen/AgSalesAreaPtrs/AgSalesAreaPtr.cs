using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "brek_list.sare_ptr")]
    public class AgSalesAreaPtr
    {
        [XmlElement(ElementName = "sare.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "sare.rating_sare_no")]
        public int RatingSalesAreaNo { get; set; }

        [XmlElement(ElementName = "sare.base_demo_no")]
        public int BaseDemoNo { get; set; }

        [XmlElement(ElementName = "sare.base_demo_no_2")]
        public int BaseDemoNo2 { get; set; }

        [XmlElement(ElementName = "short_name")]
        public string ShortName { get; set; }

        [XmlElement(ElementName = "crcy_code")]
        public string CurrencyCode { get; set; }

        [XmlElement(ElementName = "sare.required")]
        public int SalesAreaNoRequired { get; set; }

        [XmlElement(ElementName = "sare.start_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "sare.nbr_hours")]
        public int NbrHours { get; set; }
    }
}
