using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class SmoothFailures_ByRunId
        : AbstractIndexCreationTask<SmoothFailure>
    {
        public static string DefaultIndexName => "SmoothFailures/ByRunId";

        public SmoothFailures_ByRunId()
        {
            string name = IndexName;
            Map = smoothFailures => from smoothFailure in smoothFailures
                                    select new
                                    {
                                        smoothFailure.RunId
                                    };
        }
    }
}
