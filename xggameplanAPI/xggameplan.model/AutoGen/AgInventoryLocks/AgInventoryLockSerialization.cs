using System.Collections.Generic;
using System.Xml.Serialization;
using xggameplan.Model.AutoGen;

namespace xggameplan.model.AutoGen.AgInventoryLocks
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgInventoryLockSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgInventoryLock> AgInventoryLocks { get; set; }

        /// <summary>
        /// Populate dynamic AgInventoryLocks Serialization
        /// </summary>
        /// <param name="agInventoryLocks"></param>
        /// <returns></returns>
        public static AgInventoryLockSerialization MapFrom(List<AgInventoryLock> agInventoryLocks)
        {
            return new AgInventoryLockSerialization
            {
                AgInventoryLocks = agInventoryLocks,
                Size = agInventoryLocks?.Count ?? 0
            };
        }
    }
}
