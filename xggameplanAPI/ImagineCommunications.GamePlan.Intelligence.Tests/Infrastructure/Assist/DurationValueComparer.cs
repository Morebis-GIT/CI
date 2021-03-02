using NodaTime;
using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Assist
{
    public class DurationValueComparer : IValueComparer
    {
        private readonly DurationValueRetriever _durationValueRetriever;

        public DurationValueComparer(DurationValueRetriever durationValueRetriever)
        {
            _durationValueRetriever = durationValueRetriever;
        }

        public bool CanCompare(object actualValue)
        {
            return actualValue is Duration;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            if (_durationValueRetriever.IsValidDuration(expectedValue))
            {
                var duration = _durationValueRetriever.GetValue(expectedValue);
                return duration.Equals(actualValue);
            }

            return false;
        }
    }
}
