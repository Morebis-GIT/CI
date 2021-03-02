using System;
using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgProgTxDetail : IEquatable<AgProgTxDetail>
    {
        /// <summary>
        /// Programme no
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.prog_no")]
        public int ProgrammeNo { get; set; }

        /// <summary>
        /// Episode no. Always zero as we don't get this from Nine
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.epis_no")]
        public int EpisodeNo { get; set; }

        /// <summary>
        /// Sales Area Number
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.sare_no")]
        public int SalesAreaNo { get; set; }

        /// <summary>
        /// Region, same as Sales Area for non-regional systems
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.treg_no")]
        public int TregNo { get; set; }

        /// <summary>
        /// Date; YYYYMMDD
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.prgt_txmn_date")]
        public string TxDate { get; set; }

        /// <summary>
        /// Start Time; HHMMSS
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.prgt_sched_stt_time")]
        public string ScheduledStartTime { get; set; }

        /// <summary>
        /// End Time; HHMMSS
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.sched_end_time")]
        public string ScheduledEndTime { get; set; }

        /// <summary>
        /// Programme Category of the Programme. For example, if there are two categories for the same programme then
        /// there needs to be two entries in the file for the programme, everything the same except category no.
        /// </summary>
        [XmlElement(ElementName = "prgt_detail_data.new_prgc_no")]
        public int ProgCategoryNo { get; set; }

        /// <summary>
        /// Programme Classification Code
        /// </summary>
        [XmlElement(ElementName = "prog_class_code")]
        public string ClassCode { get; set; }

        /// <summary>
        /// Live Broadcast Y/N
        /// </summary>
        [XmlElement(ElementName = "live_broadcast")]
        public string LiveBroadcast { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgProgTxDetail"/></summary>
        public AgProgTxDetail Clone() => (AgProgTxDetail)MemberwiseClone();

        public bool Equals(AgProgTxDetail other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ProgrammeNo == other.ProgrammeNo && EpisodeNo == other.EpisodeNo &&
                   SalesAreaNo == other.SalesAreaNo && TregNo == other.TregNo && TxDate == other.TxDate &&
                   ScheduledStartTime == other.ScheduledStartTime && ScheduledEndTime == other.ScheduledEndTime &&
                   ProgCategoryNo == other.ProgCategoryNo && ClassCode == other.ClassCode &&
                   LiveBroadcast == other.LiveBroadcast;
        }

        public override bool Equals(object obj)
        {
            if (obj is AgProgTxDetail other)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ProgrammeNo;
                hashCode = (hashCode * 397) ^ EpisodeNo;
                hashCode = (hashCode * 397) ^ SalesAreaNo;
                hashCode = (hashCode * 397) ^ TregNo;
                hashCode = (hashCode * 397) ^ (TxDate != null ? TxDate.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ScheduledStartTime != null ? ScheduledStartTime.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ScheduledEndTime != null ? ScheduledEndTime.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ProgCategoryNo;
                hashCode = (hashCode * 397) ^ (ClassCode != null ? ClassCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LiveBroadcast != null ? LiveBroadcast.GetHashCode() : 0);

                return hashCode;
            }
        }

        public static bool operator ==(AgProgTxDetail left, AgProgTxDetail right) => Equals(left, right);

        public static bool operator !=(AgProgTxDetail left, AgProgTxDetail right) => !Equals(left, right);
    }
}
