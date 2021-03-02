using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgPassDefault
    {
        [XmlElement(ElementName = "abps_data.abpn_no")]
        public int ScenarioNo { get; set; }

        [XmlElement(ElementName = "abps_data.abdn_no")]
        public int PassNo { get; set; }

        [XmlElement(ElementName = "abps_data.sequence_no")]
        public int PassSeqNo { get; set; }

        [XmlElement(ElementName = "abps_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "abps_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "abps_data.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "abps_data.end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "abps_data.days_of_week")]
        public int DaysOfWeek { get; set; }

        [XmlElement(ElementName = "abpn_description")]
        public string ScenarioDescription { get; set; }

        [XmlElement(ElementName = "abdn_description")]
        public string PassDescription { get; set; }

        [XmlElement(ElementName = "abps_data.nbr_times")]
        public int TimeSlicesCount { get; set; }

        [XmlArray("time_list")]
        [XmlArrayItem("item")]
        public AgPassTimeSliceList TimeSliceList { get; set; }
    }

}
