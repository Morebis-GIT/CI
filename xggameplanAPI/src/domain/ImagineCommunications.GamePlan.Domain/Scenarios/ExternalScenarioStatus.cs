namespace ImagineCommunications.GamePlan.Domain.Scenarios
{
    public enum ExternalScenarioStatus
    {
        Undefined = 0,
        Accepted = 1,
        NotFound,
        Conflict,
        Completed,
        Error,
        InvalidResponse,
        Cancelled,
        Scheduled,
        Running
    }
}
