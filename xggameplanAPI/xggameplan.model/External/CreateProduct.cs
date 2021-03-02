using System;

namespace xggameplan.Model
{
    public class CreateProduct
    {
        public string Externalidentifier { get; set; }
        public string ParentExternalidentifier { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
        public string ClashCode { get; set; }
        public string AdvertiserIdentifier { get; set; }
        public string AdvertiserName { get; set; }
        public DateTime AdvertiserLinkStartDate { get; set; }
        public DateTime AdvertiserLinkEndDate { get; set; }
        public string AgencyIdentifier { get; set; }
        public string AgencyName { get; set; }
        public DateTime AgencyStartDate { get; set; }
        public DateTime AgencyLinkEndDate { get; set; }

    }

}
