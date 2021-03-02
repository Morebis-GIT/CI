using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgLengthFactors
{
    [XmlRoot(ElementName = "Item")]
    public class AgLengthFactorGroup
    {
        [XmlElement(ElementName = "lfgr_list.lfgr_no")]
        public int LengthFactorGroupNo { get; set; }

        [XmlElement(ElementName = "lfgr_list.nbr_sslg")]
        public int NbrLengthFactors { get; set; }

        [XmlArray(ElementName = "sslg_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgLengthFactorsList LengthFactors { get; set; }
    }

    public class AgLengthFactorGroupsList : List<AgLengthFactorGroup> { }
}
