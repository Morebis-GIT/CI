using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Settings
{
    /// <summary>
    /// Settings for an email notification
    /// </summary>
    public class EmailNotificationSettings
    {
        /// <summary>
        /// Whether notification is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Sender email address
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// Recipient addresses
        /// </summary>
        public List<string> RecipientAddresses { get; set; }

        /// <summary>
        /// CC addresses
        /// </summary>
        public List<string> CCAddresses { get; set; }
    }
}
