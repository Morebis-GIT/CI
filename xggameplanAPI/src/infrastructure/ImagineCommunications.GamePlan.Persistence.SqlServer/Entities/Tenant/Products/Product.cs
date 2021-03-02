using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products
{
    public class Product : IUniqueIdentifierPrimaryKey
    {
        public const string SearchFieldName = "TokenizedName";
        public static readonly IReadOnlyList<string> SearchFieldSources = new List<string>() { nameof(Externalidentifier), nameof(Name) }.AsReadOnly();
        public Guid Id { get; set; }
        public string Externalidentifier { get; set; }
        public string ParentExternalidentifier { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
        public string ClashCode { get; set; }
        public string ReportingCategory { get; set; }

        public ICollection<ProductAgency> ProductAgencies { get; set; } = new HashSet<ProductAgency>();
        public ICollection<ProductAdvertiser> ProductAdvertisers { get; set; } = new HashSet<ProductAdvertiser>();
        public ICollection<ProductPerson> ProductPersons { get; set; } = new HashSet<ProductPerson>();
    }
}
