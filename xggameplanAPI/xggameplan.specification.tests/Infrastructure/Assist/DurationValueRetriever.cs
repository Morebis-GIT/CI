using System;
using System.Collections.Generic;
using NodaTime;
using NodaTime.Text;
using TechTalk.SpecFlow.Assist;

namespace xggameplan.specification.tests.Infrastructure.Assist
{
    public class DurationValueRetriever : IValueRetriever
    {
        private static readonly IPattern<Duration> Pattern =
            DurationPattern.CreateWithInvariantCulture("-H:mm:ss.FFFFFFF");

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(Duration);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public virtual Duration GetValue(string value)
        {
            return Pattern.Parse(value).Value;
        }

        public virtual bool IsValidDuration(string value)
        {
            return Pattern.Parse(value).Success;
        }
    }
}
