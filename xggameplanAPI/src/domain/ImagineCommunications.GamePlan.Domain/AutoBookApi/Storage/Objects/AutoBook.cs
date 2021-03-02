using System;
using Raven.Imports.Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects
{
    public class AutoBook
    {
        public string Id { get; set; }

        public string Api { get; set; }

        /// <summary>
        /// Status of AutoBook instance
        /// </summary>
        public AutoBookStatuses Status { get; set; }

        public AutoBookTask Task { get; set; }

        /// <summary>
        /// Locks AutoBook for use.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Time that instance was created
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// Time that last run was started
        /// </summary>
        public DateTime LastRunStarted { get; set; }

        /// <summary>
        /// Time that last run was completed
        /// </summary>
        public DateTime LastRunCompleted { get; set; }

        /// <summary>
        /// AutoBook instance configuration
        /// </summary>
        public int InstanceConfigurationId { get; set; }

        /// <summary>
        /// Whether instance is free to receive another task
        /// </summary>
        [JsonIgnore]
        public bool IsFree
        {
            get
            {
                return !Locked && Array.IndexOf(
                    new AutoBookStatuses[] {
                        AutoBookStatuses.Task_Completed,
                        AutoBookStatuses.Task_Error,
                        AutoBookStatuses.Idle
                    }, Status) != -1;
            }
        }

        public static bool IsFreeStatus(AutoBookStatuses status)
        {
            return Array.IndexOf(
                new AutoBookStatuses[] {
                    AutoBookStatuses.Task_Completed,
                    AutoBookStatuses.Task_Error,
                    AutoBookStatuses.Idle
                }, status) != -1;
        }

        public static bool IsWorkingStatus(AutoBookStatuses status)
        {
            return Array.IndexOf(
                new AutoBookStatuses[] {
                    AutoBookStatuses.Idle,
                    AutoBookStatuses.In_Progress,
                    AutoBookStatuses.Task_Completed,
                    AutoBookStatuses.Task_Error
                }, status) != -1;
        }

        public static bool IsOKForDelete(AutoBookStatuses status)
        {
            return Array.IndexOf(
                new AutoBookStatuses[] {
                    AutoBookStatuses.Idle,
                    AutoBookStatuses.Fatal_Error
                }, status) != -1;
        }

        /// <summary>
        /// Checks if the auto book is valid for deletion. A run must be cancelled before deleting.
        /// </summary>
        /// <param name="autoBook"></param>
        public static void ValidateForDelete(AutoBook autoBook)
        {
            // Must cancel run before deleting
            if (autoBook.Status == AutoBookStatuses.In_Progress)
            {
                throw new Exception("Cannot delete AutoBook while it is processing");
            }
        }
    }
}
