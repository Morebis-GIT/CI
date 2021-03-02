using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgCampaignPriorityRound
    {
        [XmlElement(ElementName = "mode_data.round_no")]
        public int Number { get; set; }
        [XmlElement(ElementName = "mode_data.processing_mode")]
        public int ProcessingMode { get; set; }
        [XmlElement(ElementName = "mode_data.prog_reqs")]
        public int IsProgrammeInclusionsRound { get; set; }
        [XmlElement(ElementName = "mode_data.min_priority")]
        public int PriorityFrom { get; set; }
        [XmlElement(ElementName = "mode_data.max_priority")]
        public int PriorityTo { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
    }
}
