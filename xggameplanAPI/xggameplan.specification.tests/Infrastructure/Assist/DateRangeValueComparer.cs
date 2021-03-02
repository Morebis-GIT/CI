using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using TechTalk.SpecFlow.Assist;

namespace xggameplan.specification.tests.Infrastructure.Assist
{
    public class DateRangeValueComparer : IValueComparer
    {
        private readonly DateRangeValueRetriever _dateRangeValueRetriever;

        public DateRangeValueComparer(DateRangeValueRetriever dateRangeValueRetriever)
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
