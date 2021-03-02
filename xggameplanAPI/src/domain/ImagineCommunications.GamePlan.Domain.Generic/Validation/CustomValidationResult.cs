namespace ImagineCommunications.GamePlan.Domain.Generic.Validation
{
    public class CustomValidationResult
    {
        public bool Successful { get; }
        public string Message { get; }

        private CustomValidationResult(bool success, string message = null)
        {
            Successful = success;
            Message = message;
        }

        public static CustomValidationResult Success() => new CustomValidationResult(true);
        public static CustomValidationResult Failed(string message = "") => new CustomValidationResult(false, message);
    }
}
