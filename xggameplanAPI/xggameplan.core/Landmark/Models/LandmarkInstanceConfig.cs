using System;

namespace xggameplan.core.Landmark
{
    public class LandmarkInstanceConfig
    {
        private const int DefaultProbePort = 89;

        public LandmarkInstanceConfig()
        {
            ProbeUri = new Lazy<Uri>(GetProbeUri);
        }

        public Uri BaseUri { get; set; }
        public string OrganizationCode { get; set; }
        public string PositionCode { get; set; }
        public string LmkApiKey { get; set; }
        public string LmkEnvironment { get; set; }
        public string Timezone { get; set; }
        public int? ProbePort { get; set; }
        public bool IsValid => BaseUri != null &&
                               !string.IsNullOrWhiteSpace(OrganizationCode) &&
                               !string.IsNullOrWhiteSpace(PositionCode) &&
                               !string.IsNullOrWhiteSpace(LmkApiKey) &&
                               !string.IsNullOrWhiteSpace(LmkEnvironment);

        public Lazy<Uri> ProbeUri { get; }

        /// <summary>
        /// Sets values for the optional fields if not already set.
        /// </summary>
        /// <param name="other">Primary instance configuration</param>
        public void UpdateOptionalValues(LandmarkInstanceConfig other)
        {
            if (string.IsNullOrWhiteSpace(OrganizationCode))
            {
                OrganizationCode = other.OrganizationCode;
            }
            if (string.IsNullOrWhiteSpace(PositionCode))
            {
                PositionCode = other.PositionCode;
            }
            if (string.IsNullOrWhiteSpace(Timezone))
            {
                Timezone = other.Timezone;
            }

            ProbePort ??= other.ProbePort;
        }

        /// <summary>
        /// Builds landmark probe URI.
        /// </summary>
        /// <returns></returns>
        private Uri GetProbeUri()
        {
            var builder = new UriBuilder(new Uri(BaseUri, "probe.aspx"))
            {
                Port = ProbePort ?? DefaultProbePort
            };

            return builder.Uri;
        }
    }
}
