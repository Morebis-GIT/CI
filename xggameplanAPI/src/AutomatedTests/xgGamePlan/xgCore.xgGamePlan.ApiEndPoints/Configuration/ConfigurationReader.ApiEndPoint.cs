namespace xgCore.xgGamePlan.ApiEndPoints
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    public partial class ConfigurationReader
    {
        /// <summary>
        /// Represents a hydrated JSON configuration file.
        /// </summary>
        private class ApiEndPoint
        {
            public string BaseAddress { get; set; }
            public string AccessToken { get; set; }
            public string ApiVersion { get; set; }
        }
    }
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
}
