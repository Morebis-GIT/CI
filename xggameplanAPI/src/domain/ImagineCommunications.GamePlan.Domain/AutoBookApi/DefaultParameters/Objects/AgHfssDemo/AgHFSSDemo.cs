using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgHfssDemo
    {
        [XmlElement(ElementName = "hfss_demo.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "hfss_demo.index_type")]
        public int IndexType { get; set; }

        [XmlElement(ElementName = "hfss_demo.base_demo_no")]
        public int BaseDemoNo { get; set; }

        [XmlElement(ElementName = "hfss_demo.index_demo_no")]
        public int IndexDemoNo { get; set; }

        [XmlElement(ElementName = "hfss_demo.brek_sched_date")]
        public string BreakScheduledDate { get; set; }
    }
}
