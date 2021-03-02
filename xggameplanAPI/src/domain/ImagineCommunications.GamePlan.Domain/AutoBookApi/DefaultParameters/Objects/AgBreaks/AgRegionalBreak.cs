using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgRegionalBreak
    {
        [XmlElement(ElementName = "bkrg.treg_no")]
        public int TregNo { get; set; }

        [XmlElement(ElementName = "bkrg.open_avail")]
        public int OpenAvail { get; set; }
    }
}
