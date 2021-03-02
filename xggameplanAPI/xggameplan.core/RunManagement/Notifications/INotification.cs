using System;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;

namespace xggameplan.RunManagement.Notifications
{
    /// <summary>
    /// Interface for AutoBook notification
    /// </summary>
    public interface INotification<T>
    {
        /// <summary>
        /// Notification for Smooth completed
        /// </summary>
        /// <param name="run"></param>
        /// <param name="success"></param>
        /// <param name="settings"></param>
        void SmoothCompleted(Run run, bool success, T settings);

        /// <summary>
        /// Notification for run completed, all scenarios
        /// </summary>
        /// <param name="run"></param>
        /// <param name="success"></param>
        /// <param name="settings"></param>
        void RunCompleted(Run run, bool success, T settings);

        /// <summary>
        /// Notification for run scenario completed
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenario"></param>
        /// <param name="success"></param>
        /// <param name="settings"></param>
        void RunCompleted(Run run, RunScenario scenario, bool success, T settings);

        /// <summary>
        /// Call Webhook to lock inventory
        /// </summary>
        /// <param name="run"></param>
        /// <param name="settings"></param>
        void InventoryLock(Run run, T settings);

        /// <summary>
        /// Call Webhook to unlock inventory and optionally choose a scenario
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        /// <param name="settings"></param>
        void InventoryUnlock(Run run, Guid? scenarioId, T settings);

    }
}
