using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    [XmlRoot(ElementName = "Item")]
    public class AgISRTimeBand
    {
        [XmlElement(ElementName = "isr_time_data.start_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "isr_time_data.end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "isr_time_data.days")]
        public int Days { get; set; }

        [XmlElement(ElementName = "incl_excl")]
        public string Exclude { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgISRTimeBand"/></summary>
        public AgISRTimeBand Clone() => (AgISRTimeBand)MemberwiseClone();
    }
}
