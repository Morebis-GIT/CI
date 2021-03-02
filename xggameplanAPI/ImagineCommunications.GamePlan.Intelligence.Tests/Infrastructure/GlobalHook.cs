using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Assist;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure
{
    [Binding]
    public static class GlobalHook
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var durationValueRetriever = new DurationValueRetriever();
            Service.Instance.ValueRetrievers.Register(durationValueRetriever);
            Service.Instance.ValueComparers.Register(new DurationValueComparer(durationValueRetriever));
            var dateRangeValueRetriever = new DateTimeRangeValueRetriever(new DateTimeValueRetriever());
            Service.Instance.ValueRetrievers.Register(dateRangeValueRetriever);
            Service.Instance.ValueComparers.Register(new DateRangeValueComparer(dateRangeValueRetriever));
            var bufferValueRetriever = new BufferValueRetriever();
            Service.Instance.ValueRetrievers.Register(bufferValueRetriever);
            Service.Instance.ValueComparers.Register(new BufferValueComparer(bufferValueRetriever));
        }
    }
}
