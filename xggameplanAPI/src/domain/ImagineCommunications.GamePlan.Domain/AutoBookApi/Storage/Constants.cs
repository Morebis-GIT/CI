namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage
{
    /// <summary>
    /// AutoBook statuses
    /// </summary>
    public enum AutoBookStatuses : short
    {
        Idle = 0,
        Provisioning = 1,
        In_Progress = 2,
        Task_Completed = 3,
        Task_Error = 4,
        Fatal_Error = 5
    }
}
