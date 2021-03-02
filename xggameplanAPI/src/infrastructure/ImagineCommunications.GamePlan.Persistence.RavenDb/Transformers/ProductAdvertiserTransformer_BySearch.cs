using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class ProductAdvertiserTransformer_BySearch
        : AbstractTransformerCreationTask<Product_BySearch.IndexedFields>
    {
        public ProductAdvertiserTransformer_BySearch()
        {
            TransformResults = results =>
                from indexfield in results
                group indexfield by new
                {
                    indexfield.AdvertiserName,
                    indexfield.AdvertiserIdentifier
                }
                into grp
                select new ProductAdvertiserModel()
                {
                    AdvertiserName = grp.Key.AdvertiserName,
                    AdvertiserIdentifier = grp.Key.AdvertiserIdentifier
                };
        }
    }
}
