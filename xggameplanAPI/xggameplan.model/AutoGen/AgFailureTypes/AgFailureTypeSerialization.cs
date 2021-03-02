using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgFailureTypeSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgFailureType> AgFailureTypes { get; set; }

        /// <summary>
        /// Populate dynamic ag Failure Type Serialization
        /// </summary>
        /// <param name="agFailureTypes"></param>
        /// <returns></returns>
        public static AgFailureTypeSerialization MapFrom(List<AgFailureType> agFailureTypes)
        {
            return new AgFailureTypeSerialization()
            {
                AgFailureTypes = agFailureTypes,
                Size = agFailureTypes?.Count ?? 0
            };
        }
    }
}
