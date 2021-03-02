using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Assist
{
    public class DateRangeValueComparer : IValueComparer
    {
        private readonly DateTimeRangeValueRetriever _dateRangeValueRetriever;

        public DateRangeValueComparer(DateTimeRangeValueRetriever dateRangeValueRetriever)
        {
            _dateRangeValueRetriever = dateRangeValueRetriever;
        }

        public bool CanCompare(object actualValue) => actualValue is DateTimeRange;

        public bool Compare(string expectedValue, object actualValue)
        {
            if (!_dateRangeValueRetriever.IsValidDateRange(expectedValue))
            {
                return false;
            }

            var dateTimeRange = _dateRangeValueRetriever.GetValueOrDefault(expectedValue);
            return dateTimeRange.Equals(actualValue);
        }
    }
}
