using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgPeakStartEndTime
    {
        [XmlElement(ElementName = "agdt_data.sare_no")]
        public int SalesArea { get; set; }

        [XmlElement(ElementName = "agdt_data.aper_no")]
        public int ScenarioNumber { get; set; }

        [XmlElement(ElementName = "agdt_data.agdt_no")]
        public int DayPartNumber { get; set; }

        [XmlElement(ElementName = "agdt_data.agdt_stt_day")]
        public int StartDayOfDayPart { get; set; }

        [XmlElement(ElementName = "agdt_data.end_day")]
        public int EndDayOfDayPart { get; set; }

        [XmlElement(ElementName = "agdt_data.agdt_stt_time")]
        public string StartTimeOfDayPart { get; set; }

        [XmlElement(ElementName = "agdt_data.end_time")]
        public string EndTimeOfDayPart { get; set; }

        [XmlElement(ElementName = "agdt_data.mid_point")]
        public int MidPoint { get; set; }

        [XmlElement(ElementName = "agdt_desc")]
        public string Name { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgPeakStartEndTime"/></summary>
        public AgPeakStartEndTime Clone() => (AgPeakStartEndTime)MemberwiseClone();
    }
}
