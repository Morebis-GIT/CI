namespace xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios
{
    public enum ScenarioStatus
    {
        Pending = 0,
        Scheduled = 1,
        Starting = 2,
        Smoothing = 8,
        InProgress = 3,
        GettingResults = 4,
        CompletedSuccess = 5,
        CompletedError = 6,
        Deleted = 7
    }
}
