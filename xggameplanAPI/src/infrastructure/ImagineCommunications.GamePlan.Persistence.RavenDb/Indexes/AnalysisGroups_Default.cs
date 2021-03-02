using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class AnalysisGroups_Default : AbstractIndexCreationTask<AnalysisGroup>
    {
        public static string DefaultIndexName =>
            "AnalysisGroups/Default";

        public AnalysisGroups_Default()
        {
            Map = analysisGroups =>
                from analysisGroup in analysisGroups
                select new
                {
                    analysisGroup.Id,
                    analysisGroup.Name,
                    analysisGroup.IsDeleted
                };
        }
    }
}
