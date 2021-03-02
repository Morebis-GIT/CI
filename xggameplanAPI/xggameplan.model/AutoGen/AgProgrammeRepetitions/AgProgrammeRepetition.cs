using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgProgrammeRepetition
    {
        [XmlElement(ElementName = "abpr_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "abpr_data.minutes")]
        public int Minutes { get; set; }

        [XmlElement(ElementName = "abpr_data.repetition_factor")]
        public double RepetitionFactor { get; set; }

        [XmlElement(ElementName = "abpr_data.repetition_factor_pk")]
        public double PeakRepetitionFactor { get; set; }

        [XmlElement(ElementName = "abpr_data.repetition_factor_opk")]
        public double OffPeakRepetitionFactor { get; set; }
    }
}
