using System.Linq;
using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "Item")]
    public class AgDayPart
    {
        [XmlElement(ElementName = "dcdp_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "dcdp_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "dcdp_data.dcdp_no")]
        public int DayPartNo { get; set; }

        [XmlElement(ElementName = "dcdp_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "dcdp_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "dcdp_data.reqm")]
        public AgRequirement AgDayPartRequirement { get; set; }

        [XmlElement(ElementName = "dcdp_name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "dcdp_data.nbr_dcdts")]
        public int NbrAgTimeSlices { get; set; }

        [XmlElement(ElementName = "dcdp_data.spotmax_ratings")]
        public int SpotMaxRatings { get; set; }

        [XmlElement(ElementName = "dcdp_data.camp_price")]
        public decimal CampaignPrice { get; set; }

        [XmlArray("dcdt_list")]
        [XmlArrayItem("item")]
        public AgTimeSlices AgTimeSlices { get; set; }

        [XmlIgnore]
        public int NbrAgDayPartLengths { get; set; }

        [XmlIgnore]
        public AgDayPartLengths AgDayPartLengths { get; set; }

        [XmlIgnore]
        public string UniqueTimeSlicesHash =>
            string.Join(":", AgTimeSlices?
                .OrderBy(x => x.StartDay)
                .ThenBy(x => x.EndDay)
                .ThenBy(x => x.StartTime)
                .ThenBy(x => x.EndTime)
                .Select(t => $"{t.StartDay}:{t.EndDay}:{t.StartTime}:{t.EndTime}") ?? Enumerable.Empty<string>());
    }
}
