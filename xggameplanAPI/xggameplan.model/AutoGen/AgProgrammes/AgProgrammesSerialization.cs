using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgProgrammesSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgProgrammes list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgProgramme> AgProgrammes { get; set; }

        public AgProgrammesSerialization MapFrom(List<AgProgramme> agProgrammes)
        {
            AgProgrammes = agProgrammes;
            Size = agProgrammes.Count;
            return this;
        }
    }
}
