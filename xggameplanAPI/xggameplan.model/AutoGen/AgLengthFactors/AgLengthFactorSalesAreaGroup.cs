using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgLengthFactors
{
    [XmlRoot(ElementName = "Item")]
    public class AgLengthFactorSalesAreaGroup
    {
        [XmlElement(ElementName = "lefa_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "lefa_data.nbr_lfgrs")]
        public int NbrGroups { get; set; }

        [XmlArray(ElementName = "lfgr_list")]
        [XmlArrayItem(ElementName = "item")]
        public AgLengthFactorGroupsList LengthFactorGroups { get; set; }
    }
}
