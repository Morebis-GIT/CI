using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "Item")]
    public class AgISRDemographic
    {
        [XmlElement(ElementName = "isr_demo_data.demo_no")]
        public int DemographicNo { get; set; }

        [XmlElement(ElementName = "isr_demo_data.effe_threshold")]
        public int EfficiencyThreshold { get; set; }
    }

    public class AgISRDemographics : List<AgISRDemographic> { }
}
