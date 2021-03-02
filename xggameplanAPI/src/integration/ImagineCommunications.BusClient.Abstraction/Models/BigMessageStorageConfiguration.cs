using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.BusClient.Abstraction.Models
{
    /// <summary>
    /// The big message storage configuration.
    /// </summary>
    public class BigMessageStorageConfiguration
    {
        /// <summary>
        /// Gets or sets the big message size threshold.
        /// </summary>
        /// <remarks>
        /// The bulk events whose serialized size is larger than this value will be sent as <seealso cref="IBigMessage"/>.
        /// </remarks>
        public long SerializedSizeThreshold { get; set; }

        /// <summary>
        /// Gets or sets the name of the profile.
        /// </summary>
        private string _profileName;
        public string ProfileName
        {
            get => string.IsNullOrWhiteSpace(_profileName) ? "default" : _profileName;
            set => _profileName = value;
        }

        /// <summary>
        /// Gets or sets the profiles location.
        /// </summary>
        public string ProfilesLocation { get; set; }

        /// <summary>
        /// Gets or sets the AWS region endpoint.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the AWS bucket name.
        /// </summary>
        public string BucketName { get; set; }
    }
}
