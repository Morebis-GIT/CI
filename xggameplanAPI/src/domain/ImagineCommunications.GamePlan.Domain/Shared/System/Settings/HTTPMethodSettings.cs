using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Settings
{
    /// <summary>
    /// Settings for calling a single HTTP method
    /// </summary>
    public class HTTPMethodSettings
    {
        /// <summary>
        /// Method GET, PUT, POST, DELETE etc
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// URL to call. Placeholders will be replaced at runtime
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Headers
        /// </summary>
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        /// <summary>
        /// Content template (E.g. JSON, XML etc). Placeholders will be replaced at runtime.
        /// </summary>
        public string ContentTemplate { get; set; }
    }
}
