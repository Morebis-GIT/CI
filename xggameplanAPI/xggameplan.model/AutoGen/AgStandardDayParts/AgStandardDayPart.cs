using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgStandardDayParts
{
    public class AgStandardDayPart
    {
        [XmlElement(ElementName = "stdp_data.stdp_no")]
        public int DaypartNo { get; set; }

        [XmlElement(ElementName = "stdp_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "stdp_data.order")]
        public int Order { get; set; }

        [XmlElement(ElementName = "stdp_data.nbr_sdpts")]
        public int NbrTimeslices { get; set; }

        [XmlArray("sdpt_list")]
        [XmlArrayItem("item")]
        public List<AgStandardDayPartTimeslice> Timeslices { get; set; }
    }
}
