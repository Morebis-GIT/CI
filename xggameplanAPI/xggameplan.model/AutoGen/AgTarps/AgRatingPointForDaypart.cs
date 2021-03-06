﻿using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgRatingPointForDaypart
    {
        [XmlElement(ElementName = "time_list.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "time_list.end_time")]
        public string EndTime { get; set; }

        [XmlElement(ElementName = "time_list.value")]
        public double Value { get; set; }
    }
}
