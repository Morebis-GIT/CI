using System;
using System.Collections.Generic;

namespace xggameplan.Logging
{
    /// <summary>
    /// HTTP response info
    /// </summary>
    public class ResponseInfo
    {
        public string RequestID { get; set; }
        public DateTime Timestamp { get; set; }
        public string ContentType { get; set; }
        public byte[] Body { get; set; }
        public int? StatusCode { get; set; }
        public int? ElapsedTime { get; set; }
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
    }
}
