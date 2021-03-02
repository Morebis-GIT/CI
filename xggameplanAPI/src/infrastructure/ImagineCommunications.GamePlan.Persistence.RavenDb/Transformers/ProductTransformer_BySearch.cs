using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class ProductTransformer_BySearch
        : AbstractTransformerCreationTask<Product_BySearch.IndexedFields>
    {
        public ProductTransformer_BySearch()
        {
            TransformResults = results =>
                from indexfield in results
                group indexfield by new
                {
                    indexfield.Name,
                    indexfield.Externalidentifier,
                    indexfield.AdvertiserName
                }
                into grp
                select new ProductNameModel()
                {
                    Name = grp.Key.Name,
                    Externalidentifier = grp.Key.Externalidentifier,
                    AdvertiserName = grp.Key.AdvertiserName
                };
        }
    }
}
