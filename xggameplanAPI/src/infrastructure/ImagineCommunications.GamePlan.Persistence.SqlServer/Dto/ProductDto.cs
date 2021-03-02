using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Dto
{
    public class ProductDto
    {
        public Guid Uid { get; set; }
        public string Externalidentifier { get; set; }
        public string ParentExternalidentifier { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
        public string ClashCode { get; set; }
        public string ReportingCategory { get; set; }
        public int? AdvertiserId { get; set; }
        public string AdvertiserIdentifier { get; set; }
        public string AdvertiserName { get; set; }
        public string AdvertiserShortName { get; set; }
        public DateTime? AdvertiserStartDate { get; set; }
        public DateTime? AdvertiserEndDate { get; set; }
        public int? AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string AgencyShortName { get; set; }
        public string AgencyIdentifier { get; set; }
        public int? AgencyGroupId { get; set; }
        public string AgencyGroupCode { get; set; }
        public string AgencyGroupShortName { get; set; }
        public DateTime? AgencyStartDate { get; set; }
        public DateTime? AgencyEndDate { get; set; }
        public int? PersonId { get; set; }
        public string PersonName { get; set; }
        public int? PersonIdentifier { get; set; }
        public DateTime? PersonStartDate { get; set; }
        public DateTime? PersonEndDate { get; set; }
    }
}
