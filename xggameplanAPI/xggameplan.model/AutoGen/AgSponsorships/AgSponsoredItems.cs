using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    public class AgSponsoredItems
    {
        [XmlElement(ElementName = "spon_list.nbr_sares")]
        public int SalesAreasCount { get { return SalesAreas != null ? SalesAreas.Count : 0; } set { } }

        [XmlArray("sare_list")]
        [XmlArrayItem("item")]
        public List<AgSalesArea> SalesAreas { get; set; }

        [XmlElement(ElementName = "spon_list.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "spon_list.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "spon_list.nbr_times")]
        public int DayPartsCount { get { return DayParts != null ? DayParts.Count : 0; } set { } }

        [XmlArray("time_list")]
        [XmlArrayItem("item")]
        public List<AgSponsorshipDayPart> DayParts { get; set; }

        [XmlElement(ElementName = "spon_list.prog_no")]
        public int ProgramNumber { get; set; }

    }
}
