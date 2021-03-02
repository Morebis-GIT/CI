namespace ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations
{
    /// <summary>
    /// AWS instance configuration
    /// </summary>
    public class AWSInstanceConfiguration
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Instance type (E.g. t2.small, t2.medium, t2.large etc)
        /// </summary>
        public string InstanceType { get; set; }

        /// <summary>
        /// Storage size (GB)
        /// </summary>
        public int StorageSizeGb { get; set; }

        /// <summary>
        /// Cost of instance, used for identifying the cheapest.
        /// </summary>
        public double Cost { get; set; }
    }
}
