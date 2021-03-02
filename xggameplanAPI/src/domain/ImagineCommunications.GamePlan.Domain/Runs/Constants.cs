namespace ImagineCommunications.GamePlan.Domain.Runs
{
    /// <summary>
    /// List of events for a run. At the moment we only define that ones
    /// associated with notifications because we don't use this enum for
    /// anything else.
    /// </summary>
    public enum RunEvents : byte
    {
        RunCompleted = 0,
        RunScenarioCompleted = 1,
        SmoothCompleted = 2
    }

    public enum EfficiencyCalculationPeriod
    {
        RunPeriod = 0,
        NumberOfWeeks = 1
    }

    public enum RunStatus
    {
        InProgress,
        Complete,
        Errors,
        NotStarted,
        Cancelled
    }
}
