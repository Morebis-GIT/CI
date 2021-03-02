namespace ImagineCommunications.BusClient.Abstraction.Models
{
    public class ServiceBusConfigModel
    {
        public string Uri { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public SerializerType SerializerType { get; set; }

        public RetryConfigModel RetryConfig { get; set; }
    }

    public class RetryConfigModel
    {
        public RetryType RetryType { get; set; }

        public int? RetryCount { get; set; }

        public int? IntervalSecond { get; set; }

        public int? MinInterval { get; set; }

        public int? MaxInterval { get; set; }

        public int? IntervalDelta { get; set; }

        public int? IntervalIncrement { get; set; }

        public string Intervals { get; set; }
    }

    public enum SerializerType
    {
        Json,
        Bson,
        Xml
    }

    public enum RetryType
    {
        Interval,
        Exponential,
        Immediate,
        Incremental,
        Intervals,
        None
    }
}
