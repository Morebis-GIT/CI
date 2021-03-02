using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.model.AutoGen.AgSponsorships
{
    public class AgSponsorship 
    {
        [XmlElement(ElementName = "spon_excl.nbr_spons")]
        public int SponsoredItemsCount { get; set; }

        [XmlArray("spon_list")]
        [XmlArrayItem("item")]
        public List<AgSponsoredItems> SponsoredItems { get; set; }

        [XmlElement(ElementName = "spon_excl.nbr_prods")]
        public int ProductsCount { get { return ProductsCode != null ? ProductsCode.Count : 0; } set { } }

        [XmlArray("prod_list")]
        [XmlArrayItem("item")]
        public List<AgProductCode> ProductsCode { get; set; }


        [XmlElement(ElementName = "spon_excl.calc_type")]
        public int CalculationType { get; set; }

        [XmlElement(ElementName = "spon_excl.rest_type")]
        public int RestrictionType { get; set; }

        [XmlElement(ElementName = "spon_excl.rest_value")]
        public int RestrictionValue { get; set; }

        [XmlElement(ElementName = "spon_excl.applicability")]
        public int Applicability { get; set; }

        [XmlElement(ElementName = "spon_excl.nbr_clsh_excls")]
        public int ClashExclusivitiesCount { get { return ClashExclusivities != null ? ClashExclusivities.Count : 0; } set { } }

        [XmlArray("clsh_excl_list")]
        [XmlArrayItem("item")]
        public List<AgClashExclusivity> ClashExclusivities { get; set; }


        [XmlElement(ElementName = "spon_excl.nbr_advt_excls")]
        public int AdvertiserExclusivitiesCount { get { return AdvertiserExclusivities != null ? AdvertiserExclusivities.Count : 0; } set { } }

        [XmlArray("advt_excl_list")]
        [XmlArrayItem("item")]
        public List<AgAdvertiserExclusivity> AdvertiserExclusivities { get; set; }
    }
}
