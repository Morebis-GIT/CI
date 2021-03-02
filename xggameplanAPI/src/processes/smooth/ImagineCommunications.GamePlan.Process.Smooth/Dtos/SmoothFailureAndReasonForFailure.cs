using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public class SmoothFailureAndReasonForFailure
    {
        public SmoothFailureMessages FailureMessage { get; set; }

        /// <summary>
        /// Restriction, or null if failure not related to restriction
        /// </summary>
        public Restriction Restriction { get; set; }

        /// <summary>
        /// Constructor for failure message only
        /// </summary>
        /// <param name="failureMessage"></param>
        public SmoothFailureAndReasonForFailure(SmoothFailureMessages failureMessage)
        {
            FailureMessage = failureMessage;
        }

        /// <summary>
        /// Constructor for failure message and restriction
        /// </summary>
        /// <param name="failureMessage"></param>
        /// <param name="restriction"></param>
        public SmoothFailureAndReasonForFailure(
            SmoothFailureMessages failureMessage,
            Restriction restriction)
            : this(failureMessage)
        {
            Restriction = restriction;
        }
    }
}
