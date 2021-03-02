using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class ClashTransformer_BySearch
        : AbstractTransformerCreationTask<Clash_BySearch.IndexedFields>
    {
        public ClashTransformer_BySearch()
        {
            TransformResults = results =>
                from indexfield in results
                group indexfield by new
                {
                    indexfield.Description,
                    indexfield.Externalref
                }
                into grp
                select new ClashNameModel()
                {
                    Description = grp.Key.Description,
                    Externalref = grp.Key.Externalref
                };
        }
    }
}
