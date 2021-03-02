using System.Xml.Serialization;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgProgRestriction
    {
        [XmlElement(ElementName = "preq_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "preq_data.sare_no")]
        public int SalesAreaNo { get; set; }

        [XmlElement(ElementName = "preq_data.prog_no")]
        public int ProgNo { get; set; }

        [XmlElement(ElementName = "preq_data.prgc_no")]
        public int PrgcNo { get; set; }

        [XmlElement(ElementName = "incl_excl_flag")]
        public string IncludeExcludeFlag { get; set; }

        [XmlElement(ElementName = "preq_data.epis_no")]
        public int EpisNo { get; set; }

        /// <summary>Returns a shallow copy instance of <see cref="AgProgRestriction"/></summary>
        public AgProgRestriction Clone() => (AgProgRestriction)MemberwiseClone();
    }
}
