using System;
using System.Collections.Generic;

namespace xggameplan.Logging
{
    /// <summary>
    /// HTTP request info
    /// </summary>
    public class RequestInfo
    {
        public string RequestID { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
        public string ContentType { get; set; }
        public byte[] Body { get; set; }
        public string Uri { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
    }
}
