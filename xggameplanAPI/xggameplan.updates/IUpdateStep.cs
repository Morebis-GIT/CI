using System;

namespace xggameplan.Updates
{
    /// <summary>
    /// Step within an update.
    /// </summary>
    public interface IUpdateStep
    {
        /// <summary>
        /// Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Sequence number of update
        /// </summary>
        int Sequence { get; }

        /// <summary>
        /// Name of step
        /// </summary>
        string Name { get; }       

        /// <summary>
        /// Applies the update
        /// </summary>
        void Apply ();

        /// <summary>
        /// Whether step supports roll back
        /// </summary>
        bool SupportsRollback { get; }

        /// <summary>
        /// Roll back the step
        /// </summary>
        void RollBack();
    }
}
