using System.Collections.Generic;
using System.Xml.Serialization;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgLengthFactors
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgLengthFactorSalesAreaGroupSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgLengthFactorSalesAreaGroup> AgLengthFactors { get; set; }

        /// <summary>
        /// Populate dynamic AgLengthFactorSalesAreaGroup Serialization
        /// </summary>
        /// <param name="agLengthFactorSalesAreaGroups"></param>
        /// <returns></returns>
        public static AgLengthFactorSalesAreaGroupSerialization MapFrom(List<AgLengthFactorSalesAreaGroup> agLengthFactorSalesAreaGroups) =>
            new AgLengthFactorSalesAreaGroupSerialization
            {
                AgLengthFactors = agLengthFactorSalesAreaGroups,
                Size = agLengthFactorSalesAreaGroups?.Count ?? 0
            };
    }
}
