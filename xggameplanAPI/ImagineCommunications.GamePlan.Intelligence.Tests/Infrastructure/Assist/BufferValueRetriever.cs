using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Assist
{
    public class BufferValueRetriever : IValueRetriever
    {

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(byte[]);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValueOrDefault(keyValuePair.Value);
        }

        public virtual byte[] GetValueOrDefault(string value)
        {
            var values = value.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries );
            return values.Select(Byte.Parse).ToArray();
        }

        public bool IsValidBuffer(string value)
        {
            return value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Byte.TryParse(x, out var v)).All(x => x);
        }
    }
}
