using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgProgrammeRepetitionsSerialization : BoostSerialization
    {
        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgProgrammeRepetition> AgProgrammeRepetitions { get; set; }

        /// <summary>
        /// Dynamic auto gen Programme Repetitions xml population
        /// </summary>
        /// <param name="agProgrammeRepetitions"></param>
        /// <returns></returns>
        public AgProgrammeRepetitionsSerialization MapFrom(List<AgProgrammeRepetition> agProgrammeRepetitions)
        {
            AgProgrammeRepetitions = agProgrammeRepetitions;
            Size = agProgrammeRepetitions.Count;
            return this;
        }
    }
}
