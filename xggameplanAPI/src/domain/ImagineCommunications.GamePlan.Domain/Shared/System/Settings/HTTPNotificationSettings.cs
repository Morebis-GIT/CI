using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Settings
{
    /// <summary>
    /// Settings for an HTTP notification
    /// </summary>
    public class HTTPNotificationSettings
    {
        public bool Enabled { get; set; }

        /// <summary>
        /// HTTP method
        /// </summary>
        public HTTPMethodSettings MethodSettings { get; set; }

        /// <summary>
        /// Success status codes
        /// </summary>
        public List<int> SucccessStatusCodes = new List<int>();
    }
}
