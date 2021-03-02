using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgSalesAreaDemographic
    {
        [XmlElement(ElementName = "rdem_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "rdem_data.demo_no")]
        public int DemographicNo { get; set; }

        [XmlElement(ElementName = "rdem_data.excl_eff_demo")]
        public int Exclude { get; set; }

        [XmlElement(ElementName = "rsup_code")]
        public string SupplierCode { get; set; }
    }
}
