using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace xggameplan.OutputProcessors.Abstractions
{
    public interface ILandmarkOutputFileProcessor
    {
        void ProcessFile(string file, ScenarioResult scenarioResult);
    }
}
