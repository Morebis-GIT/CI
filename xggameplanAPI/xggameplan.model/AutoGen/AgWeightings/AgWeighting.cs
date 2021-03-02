using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgWeighting
    {
        [XmlElement(ElementName = "abwe_data.aper_no")]
        public int ScenarioNo { get; set; }

        [XmlElement(ElementName = "abwe_data.abwe_type")]
        public int WeightingNo { get; set; }

        [XmlElement(ElementName = "abwe_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "abwe_data.value")]
        public int Value { get; set; }
    }
}
