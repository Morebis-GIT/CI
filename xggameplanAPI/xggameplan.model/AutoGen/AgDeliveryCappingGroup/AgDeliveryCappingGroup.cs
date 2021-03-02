using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgDeliveryCappingGroup
    {
        [XmlElement(ElementName = "abdg_data.abdg_no")]
        public int Id { get; set; }

        [XmlElement(ElementName = "abdg_data.percentage")]
        public int Percentage { get; set; }

        [XmlElement(ElementName = "abdg_data.apply_to_price")]
        public int ApplyToPrice { get; set; }
    }
}
