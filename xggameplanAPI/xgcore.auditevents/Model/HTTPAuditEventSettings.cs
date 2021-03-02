using System.Collections.Generic;
using System.Net;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Audit event settings for HTTP
    /// </summary>
    public class HTTPAuditEventSettings
    {
        public int EventTypeId { get; set; }

        public bool Enabled { get; set; }

        public HTTPRequestSettings RequestSettings { get; set; }

        public HTTPResponseSettings ResponseSettings { get; set; }
    }

    /// <summary>
    /// Settings for sending REST request
    /// </summary>
    public class HTTPRequestSettings
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

    /// <summary>
    /// Settings for handling a response to an REST request
    /// </summary>
    public class HTTPResponseSettings
    {
        /// <summary>
        /// HTTP status codes that indicate success
        /// </summary>
        public List<HttpStatusCode> SuccessStatusCodes = new List<HttpStatusCode>();
    }
}
