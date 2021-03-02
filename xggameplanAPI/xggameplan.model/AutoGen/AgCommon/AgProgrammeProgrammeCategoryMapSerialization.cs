using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgProgrammeProgrammeCategoryMapSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public AgProgrammeProgrammeCategoryMappings AgProgrammeProgrammeCategoryMaps { get; set; }

        public AgProgrammeProgrammeCategoryMapSerialization MapFrom(AgProgrammeProgrammeCategoryMappings mappings)
        {
            Size = mappings.Count;
            AgProgrammeProgrammeCategoryMaps = mappings;

            return this;
        }
    }
}
