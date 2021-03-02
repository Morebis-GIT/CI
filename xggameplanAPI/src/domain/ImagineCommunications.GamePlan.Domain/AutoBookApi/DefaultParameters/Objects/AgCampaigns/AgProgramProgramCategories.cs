using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgCampaignProgrammeProgrammeCategory
    {
        [XmlElement(ElementName = "prog_list_data.prog_no")]
        public int ProgrammeNumber { get; set; }

        [XmlElement(ElementName = "prog_list_data.prgc_no")]
        public int CategoryNumber { get; set; }
    }
}
