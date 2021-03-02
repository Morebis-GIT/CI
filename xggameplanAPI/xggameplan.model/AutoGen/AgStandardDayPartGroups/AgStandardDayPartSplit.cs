using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgStandardDayPartGroups
{
    [XmlRoot(ElementName = "Item")]
    public class AgStandardDayPartSplit
    {
        [XmlElement(ElementName = "stdp_list.stdp_no")]
        public int DayPartNo { get; set; }

        [XmlElement(ElementName = "stdp_list.perc")]
        public double Split { get; set; }
    }
}
