using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgInventoryLocks
{
    public class AgInventoryLock
    {
        [XmlElement(ElementName = "invs_data.sare_no")]
        public int SalesArea { get; set; }

        [XmlElement(ElementName = "invh_code")]
        public string InventoryCode { get; set; }

        [XmlElement(ElementName = "invs_data.lock_type")]
        public int LockType { get; set; }

        [XmlElement(ElementName = "invs_data.stt_date")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "invs_data.end_date")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "invs_data.stt_time")]
        public string StartTime { get; set; }

        [XmlElement(ElementName = "invs_data.end_time")]
        public string EndTime { get; set; }
    }
}
