using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace xggameplan.specification.tests.Infrastructure.Assist
{
    public class DateRangeValueRetriever : IValueRetriever
    {
        private readonly DateTimeValueRetriever _dateTimeValueRetriever;

        public DateRangeValueRetriever(DateTimeValueRetriever dateTimeValueRetriever)
        {
            _dateTimeValueRetriever = dateTimeValueRetriever;
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(DateTimeRange);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValueOrDefault(keyValuePair.Value);
        }

        public virtual DateTimeRange GetValueOrDefault(string value)
        {
            var success = TryParse(value, out var dateRange);
            return success ? dateRange : default;
        }

        public virtual bool IsValidDateRange(string value)
        {
            return TryParse(value, out _);
        }

        private bool TryParse(string value, out DateTimeRange result)
        {
            var dates = value
                .Split(new[] {"to"}, StringSplitOptions.None)
                .Where(dateString => DateTime.TryParse(dateString, out _))
                .Select(dateString => _dateTimeValueRetriever.GetValue(dateString))
                .ToList();

            if (dates.Count != 2)
            {
                result = default;
                return false;
            }

            result = (dates[0], dates[1]);
            return true;
        }
    }
}
