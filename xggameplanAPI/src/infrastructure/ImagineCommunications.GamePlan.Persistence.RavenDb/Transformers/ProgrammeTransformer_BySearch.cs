using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class ProgrammeTransformer_BySearch
        : AbstractTransformerCreationTask<Programmes_ByIdAndSalesAreaStartDateTime.IndexedFields>
    {

        public ProgrammeTransformer_BySearch()
        {
            TransformResults = results =>
                from indexField in results
                group indexField by new
                {
                    indexField.ProgrammeName,
                    indexField.ExternalReference
                }
                into grp
                select new ProgrammeNameModel()
                {
                    ProgrammeName = grp.Key.ProgrammeName,
                    ExternalReference = grp.Key.ExternalReference
                };
        }
    }
}
