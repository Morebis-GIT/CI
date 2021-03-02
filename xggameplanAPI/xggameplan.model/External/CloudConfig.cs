namespace xggameplan.Model
{
    public enum CloudStorageType
    {
        S3 = 0,
        Azure = 1
    };

    /// <summary>
    /// Cloud storage config includes S3 and Azure
    /// </summary>
    public class CloudConfig
    {
        /// <summary>
        /// Type of storage S3/Azure/...
        /// </summary>
        public CloudStorageType StorageType { get; set; }
        /// <summary>
        /// If Aws storage then Config
        /// </summary>
        public AwsConfiguration AwsConfig { get; set; }
    }

    /// <summary>
    /// S3 storage config
    /// </summary>
    public class AwsConfiguration
    {
        /// <summary>
        /// ProfileName
        /// </summary>
        public string Profile { get; set; }
        /// <summary>
        /// Profile stored location(credential file)
        /// </summary>
        public string ProfilesLocation { get; set; }
        /// <summary>
        /// AWS RegionEnd point
        /// </summary>
        public string Region { get; set; }
    }
}
