using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace xggameplan.AuditEvents
{
    [Serializable]
    public class ExceptionModel
    {
        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("StackTrace")]
        public string StackTrace { get; set; }

        [JsonProperty("Source")]
        public string Source { get; set; }

        [JsonProperty("HResult")]
        public int HResult { get; set; }

        [JsonProperty("HelpLink")]
        public string HelpLink { get; set; }

        //[JsonProperty("TargetSite")]
        //public string TargetSite { get; set; }

        [JsonProperty("InnerException")]
        public ExceptionModel InnerException { get; set; }

        [JsonProperty("Data")]
        public Dictionary<string, object> Data = new Dictionary<string, object>();

        public static ExceptionModel MapFrom(Exception exception)
        {
            ExceptionModel exceptionDetails = new ExceptionModel()
            {
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Source = exception.Source,
                HResult = exception.HResult,
                HelpLink = exception.HelpLink
            };

            // Serialize data
            if (exception.Data != null)
            {
                foreach (var key in exception.Data.Keys)
                {
                    if (IsSerializableData(exception.Data[key]))
                    {
                        exceptionDetails.Data.Add(key.ToString(), exception.Data[key]);
                    }
                }
            }

            if (exception.InnerException != null)
            {
                exceptionDetails.InnerException = ExceptionModel.MapFrom(exception.InnerException);
            }
            return exceptionDetails;
        }

        private static bool IsSerializableData(object data)
        {
            return data != null && IsBaseType(data.GetType());
        }

        private static bool IsBaseType(Type type)
        {
            return Array.IndexOf(new Type[] { typeof(byte), typeof(char), typeof(bool), typeof(decimal), typeof(double), typeof(short),
                                typeof(int), typeof(long), typeof(Guid), typeof(float), typeof(DateTime), typeof(Single), typeof(string),
                                typeof(ushort), typeof(uint), typeof(ulong), typeof(sbyte) }, type) != -1;
        }
    }
}
