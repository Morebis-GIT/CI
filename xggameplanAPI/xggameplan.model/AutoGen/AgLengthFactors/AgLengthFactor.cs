using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgLengthFactors
{
    [XmlRoot(ElementName = "Item")]
    public class AgLengthFactor
    {
        [XmlElement(ElementName = "sslg_list.sslg_length")]
        public long Duration { get; set; }

        [XmlElement(ElementName = "sslg_list.price_factor")]
        public double Factor { get; set; }
    }

    public class AgLengthFactorsList : List<AgLengthFactor> { }
}
