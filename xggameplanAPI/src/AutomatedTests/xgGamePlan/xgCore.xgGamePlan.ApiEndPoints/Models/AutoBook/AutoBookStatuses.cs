namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    public enum AutoBookStatuses
    {
        Idle = 0,
        Provisioning = 1,
        In_Progress = 2,
        Task_Completed = 3,
        Task_Error = 4,
        Fatal_Error = 5
    }
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
#pragma warning restore CA1707 // Identifiers should not contain underscores
}
