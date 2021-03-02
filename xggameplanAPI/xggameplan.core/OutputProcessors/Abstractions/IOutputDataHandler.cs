using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;

namespace xggameplan.OutputProcessors.Abstractions
{
    public interface IOutputDataHandler<in TData>
        where TData : class
    {
        void ProcessData(TData data, Run run, Scenario scenario);
    }
}
