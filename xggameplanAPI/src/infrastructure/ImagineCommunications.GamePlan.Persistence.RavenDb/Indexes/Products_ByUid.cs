using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public class Products_ByUid
        : AbstractIndexCreationTask<Product>
#pragma warning restore CA1707 // Identifiers should not contain underscores
    {
        public static string DefaultIndexName => "Products/ByUid";

        public Products_ByUid()
        {
            Map = products =>
                from product in products
                select new
                {
                    product.Uid
                };
        }
    }
}
