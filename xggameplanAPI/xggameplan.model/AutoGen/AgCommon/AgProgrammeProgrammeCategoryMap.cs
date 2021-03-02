using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgProgrammeProgrammeCategoryMap
    {
        [XmlElement(ElementName = "prog_prgc_data.prog_no")]
        public int ProgrammeNumber { get; set; }

        [XmlElement(ElementName = "prog_prgc_data.prgc_no")]
        public int CategoryNumber { get; set; }
    }

    public class AgProgrammeProgrammeCategoryMappings : List<AgProgrammeProgrammeCategoryMap> { }
}
