using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgProgramme
    {
        [XmlElement(ElementName = "rep_prgt_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "rep_prgt_data.week_no")]
        public int WeekNo { get; set; }

        [XmlElement(ElementName = "rep_prgt_data.week_stt_date")]
        public string WeekStartDate { get; set; }

        [XmlElement(ElementName = "rep_prgt_data.week_end_date")]
        public string WeekEndDate { get; set; }

        [XmlElement(ElementName = "rep_prgt_data.prog_no")]
        public int ProgrammeNo { get; set; }

        [XmlElement(ElementName = "short_name")]
        public string ShortName { get; set; }

        [XmlElement(ElementName = "rep_prgt_data.total_minutes")]
        public int TotalMinutes { get; set; }
    }
}
