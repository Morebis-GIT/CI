using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.Products.Objects
{
    public class Product
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public Guid Uid { get; set; }
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
        public string ReportingCategory { get; set; }
        public SalesExecutive SalesExecutive { get; set; }
        public AgencyGroup AgencyGroup { get; set; }

        public override string ToString() => $"{Name} {AdvertiserName ?? "[NoAdvertiserName]"}";
    }
}
