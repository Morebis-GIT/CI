using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgRule
    {
        [XmlElement(ElementName = "agru_data.aper_no")]
        public int ScenarioNo { get; set; }

        [XmlElement(ElementName = "agru_data.agru_type")]
        public int SlottingControlNo { get; set; }

        [XmlElement(ElementName = "agru_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "agru_data.value")]
        public double Value { get; set; }

        [XmlElement(ElementName = "agru_data.peak_value")]
        public double PeakValue { get; set; }

        [XmlElement(ElementName = "agru_data.offpeak_value")]
        public double OffPeakValue { get; set; }

        /// <summary>
        /// Ignore current slotting controls rule
        /// </summary>
        [XmlElement(ElementName = "agru_data.ignore_tf")]
        public int Ignore { get; set; }
    }
}
