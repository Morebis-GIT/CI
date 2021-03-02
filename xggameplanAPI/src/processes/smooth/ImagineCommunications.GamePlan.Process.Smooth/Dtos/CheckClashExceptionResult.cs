using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Result for checking clash exception
    /// </summary>
    public class CheckClashExceptionResult
    {
        public ClashException ClashException { get; }

        public CheckClashExceptionResult(ClashException clashException) =>
            ClashException = clashException;
    }
}
