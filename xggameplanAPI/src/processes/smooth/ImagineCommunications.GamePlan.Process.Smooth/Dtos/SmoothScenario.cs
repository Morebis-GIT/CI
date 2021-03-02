using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Smooth scenario
    /// </summary>
    internal class SmoothScenario
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Sequence
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Priority. If two scenarios are equally the best then this allows us
        /// to pick the one with the highest. E.g. If two scenarios both move
        /// spots from a break then the importance of the spots would dictate
        /// the impact.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Actions to execute
        /// </summary>
        public List<SmoothAction> Actions = new List<SmoothAction>();
    }
}
