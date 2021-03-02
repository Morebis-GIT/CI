namespace ImagineCommunications.GamePlan.Domain.RatingSchedules
{
    public class RatingsPredictionValidationMessage
    {
        public string Message { get; set; }

        public ValidationSeverityLevel SeverityLevel { get; set; }
    }

    public enum ValidationSeverityLevel
    {
        Error,
        Warning
    }
}
