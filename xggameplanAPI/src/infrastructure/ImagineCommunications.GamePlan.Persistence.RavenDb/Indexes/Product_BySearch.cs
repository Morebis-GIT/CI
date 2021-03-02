using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
#pragma warning disable CA1707 // Identifiers should not contain underscores

    public class Product_BySearch
        : AbstractIndexCreationTask<Product, Product_BySearch.IndexedFields>
#pragma warning restore CA1707 // Identifiers should not contain underscores
    {
        public static string DefaultIndexName => "Product/BySearch";

        public class IndexedFields
        {
            public string TokenizedName { get; set; }

            public string Name { get; set; }

            public string Externalidentifier { get; set; }

            public string ClashCode { get; set; }

            public DateTime EffectiveStartDate { get; set; }

            public DateTime EffectiveEndDate { get; set; }

            public string TokenizedAdvertiser { get; set; }

            public string AdvertiserIdentifier { get; set; }

            public string AdvertiserName { get; set; }

            public string AgencyName { get; set; }

            public string AgencyIdentifier { get; set; }

            public string AgencyGroup_Code { get; set; }

            public int SalesExecutive_Identifier { get; set; }

            public string ReportingCategory { get; set; }
        }

        public Product_BySearch()
        {
            var name = IndexName;
            Map = products =>
                from p in products
                select new
                {
                    TokenizedName = $"{p.Externalidentifier} {p.Name}",
                    Name = p.Name,
                    Externalidentifier = p.Externalidentifier,
                    ClashCode = p.ClashCode,
                    p.EffectiveStartDate,
                    p.EffectiveEndDate,
                    TokenizedAdvertiser = $"{p.AdvertiserIdentifier} {p.AdvertiserName}",
                    AdvertiserIdentifier = p.AdvertiserIdentifier,
                    AdvertiserName = p.AdvertiserName,
                    AgencyName = p.AgencyName,
                    AgencyIdentifier = p.AgencyIdentifier,
                    AgencyGroup_Code = p.AgencyGroup.Code,
                    SalesExecutive_Identifier = p.SalesExecutive.Identifier,
                    ReportingCategory = p.ReportingCategory
                };

            Index(p => p.TokenizedName, FieldIndexing.Analyzed);
            Index(p => p.TokenizedAdvertiser, FieldIndexing.Analyzed);
            Index(p => p.ClashCode, FieldIndexing.Analyzed);
            Index(p => p.AdvertiserName, FieldIndexing.Analyzed);
            Index(p => p.Name, FieldIndexing.Analyzed);
            Index(p => p.AgencyName, FieldIndexing.Analyzed);
            Index(p => p.AgencyIdentifier, FieldIndexing.Analyzed);
            Index(p => p.AgencyGroup_Code, FieldIndexing.Analyzed);
            Index(p => p.SalesExecutive_Identifier, FieldIndexing.Analyzed);
            Index(p => p.ReportingCategory, FieldIndexing.Analyzed);
        }
    }
}
