using System;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments
{
    /// <summary>
    /// Defines an event that reports restriction checking progress.
    /// </summary>
    public class RestrictionCheckProgressEventArgs
        : EventArgs
    {
        public string Message { get; }

        public RestrictionCheckProgressEventArgs(string message) =>
            Message = message;
    }
}
