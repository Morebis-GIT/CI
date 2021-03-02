using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgInventoryStatusesSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgInventoryStatus> AgInventoryStatuses { get; set; }

        /// <summary>
        /// Populate dynamic AgInventoryStatuses Serialization
        /// </summary>
        /// <param name="agInventoryStatuses"></param>
        /// <returns></returns>
        public AgInventoryStatusesSerialization MapFrom(List<AgInventoryStatus> agInventoryStatuses)
        {
            return new AgInventoryStatusesSerialization
            {
                AgInventoryStatuses = agInventoryStatuses,
                Size = agInventoryStatuses?.Count ?? 0
            };
        }
    }
}
