using ImagineCommunications.BusClient.Abstraction.Models;

namespace ImagineCommunications.Intelligence.Test.Api
{
    public static class AppSettingsConfig
    {
        public static SerializerType GetSerializerType(string type)
        {
            switch (type)
            {
                case "Json": return SerializerType.Json;
                case "Bson": return SerializerType.Bson;
                case "Xml": return SerializerType.Xml;
                default: return SerializerType.Json;
            }
        }

        public static RetryType GetRetryType(string type)
        {
            switch (type)
            {
                case "Interval": return RetryType.Interval;
                case "Exponential": return RetryType.Exponential;
                case "Immediate": return RetryType.Immediate;
                case "Incremental": return RetryType.Incremental;
                case "Intervals": return RetryType.Intervals;
                case "None": return RetryType.None;
                default: return RetryType.None;
            }
        }
    }
}
