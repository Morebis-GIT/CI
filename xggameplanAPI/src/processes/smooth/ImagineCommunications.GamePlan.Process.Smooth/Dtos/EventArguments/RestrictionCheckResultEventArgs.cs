using System;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    /// <summary>
    /// Defines an event that is raised when the restriction checker has
    /// determined if a restriction applies or not. The <see cref="Message"/>
    /// will contain details of the decision made.
    /// </summary>
    public class RestrictionCheckResultEventArgs
        : EventArgs
    {
        public string Message { get; }
        public RestrictionReasons Reason { get; }

        public RestrictionCheckResultEventArgs(string message, RestrictionReasons reason) =>
            (Reason, Message) = (reason, message);
    }
}
