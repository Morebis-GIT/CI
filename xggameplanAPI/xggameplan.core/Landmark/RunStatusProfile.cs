using System;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.core.Landmark
{
    internal class RunStatusProfile : AutoMapper.Profile
    {
        public RunStatusProfile()
        {
            CreateMap<ExternalScenarioStatus, RunStatus>()
                .ConvertUsing((status, d) =>
                {
                    switch (status)
                    {
                        case ExternalScenarioStatus.Accepted: return RunStatus.InProgress;
                        case ExternalScenarioStatus.Completed: return RunStatus.Complete;
                        case ExternalScenarioStatus.NotFound: return RunStatus.Errors;
                        case ExternalScenarioStatus.Conflict: return RunStatus.Errors;
                        case ExternalScenarioStatus.Error: return RunStatus.Errors;
                        case ExternalScenarioStatus.InvalidResponse: return RunStatus.Errors;
                        case ExternalScenarioStatus.Cancelled: return RunStatus.Cancelled;
                        default:
                            throw new InvalidOperationException($"Unknown external scenario status {status}");
                    }
                });
        }
    }
}
