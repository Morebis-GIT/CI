namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes.Interfaces
{
    public interface IMockEventBase
    {
        int Id { get; set; }
        bool IsModelValid { get; set; }
        bool BusinessValidationPassed { get; set; }
    }
}
