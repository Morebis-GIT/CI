using System;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.DomainLogic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using NodaTime;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Core.Tests.GherkinTests
{
    [Binding]
    public class CalculateClashExceptionCountSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly Fixture _fixture = new SafeFixture();

        private const string TestResultKey = "TestResultKey";
        private const string TestBreakKey = "TestBreakKey";
        private const string TestClashKey = "TestClashKey";
        private const string TestPeakKey = "TestPeakKey";

        public CalculateClashExceptionCountSteps(ScenarioContext scenarioContext) =>
            _scenarioContext = scenarioContext;

        [BeforeTestRun]
        public static void RegisterCustomTypes()
        {
            Service.Instance.ValueRetrievers.Register<TimeAndDowValueRetriever>();
        }

        [BeforeScenario]
        public void CacheClear() =>
            _scenarioContext.Clear();

        private string ToCacheKey(string value) =>
            value + _scenarioContext.ScenarioInfo.Title.GetHashCode().ToString();

        private T CacheRead<T>(string key) =>
            _scenarioContext.TryGetValue(ToCacheKey(key), out T item)
                ? item
                : throw new InvalidOperationException($"Could not find the key {key} in the scenario cache");

        private void CacheWrite<T>(string key, T item) =>
            _scenarioContext.Set(item, ToCacheKey(key));

        [Given("a new break")]
        public void GivenANewBreak()
        {
            CacheWrite(TestBreakKey, _fixture.Create<Break>());
        }

        [Given("a new clash")]
        public void GivenANewClash()
        {
            CacheWrite(TestClashKey, _fixture.Create<Clash>());
        }

        [Given("the break is in sales area '(.*)'")]
        public void GivenTheBreakIsInSalesArea(string salesArea)
        {
            var testBreak = CacheRead<Break>(TestBreakKey);
            testBreak.SalesArea = salesArea;
            CacheWrite(TestBreakKey, testBreak);
        }

        [Given("a peak time running from '(.*)' to '(.*)'")]
        public void GivenAPeakTimeRunningFromTo(string peakStart, string peakEnd)
        {
            _ = TimeSpan.TryParse(peakStart, out TimeSpan pST) ?
                pST :
                throw new ArgumentException("The peak start time is not valid", nameof(peakStart));

            _ = TimeSpan.TryParse(peakEnd, out TimeSpan pET) ?
                pET :
                throw new ArgumentException("The peak end time is not valid", nameof(peakEnd));

            CacheWrite(TestPeakKey, (peakStart, peakEnd));
        }

        [Given("no peak time")]
        public void GivenNoPeakTime()
        {
            CacheWrite(TestPeakKey, (String.Empty, String.Empty));
        }

        [Given("a break in sales area '(.*)'")]
        public void GivenABreakInSalesArea(string breakSalesArea)
        {
            var testBreak = CacheRead<Break>(TestBreakKey);
            testBreak.SalesArea = breakSalesArea;
            CacheWrite(TestBreakKey, testBreak);
        }

        [Given("the break is (.*) minutes long")]
        public void GivenTheBreakIsMinutesLong(int breakDurationMinutes)
        {
            var testBreak = CacheRead<Break>(TestBreakKey);
            testBreak.Duration = Duration.FromMinutes(breakDurationMinutes);
            CacheWrite(TestBreakKey, testBreak);
        }

        [Given("the break starts at '(.*)'")]
        public void GivenTheBreakStartsAt(string breakStart)
        {
            var parsedBreakStart = DateTime.TryParse(breakStart, out DateTime start) ?
                start :
                throw new ArgumentException("The break date and/or time is not valid", nameof(breakStart));

            var testBreak = CacheRead<Break>(TestBreakKey);
            testBreak.ScheduledDate = parsedBreakStart;
            CacheWrite(TestBreakKey, testBreak);
        }

        [Given("the clash has default off peak exposure count of (.*)")]
        public void GivenTheClashHasDefaultOffPeakExposureCountOf(int exposureCount)
        {
            var testClash = CacheRead<Clash>(TestClashKey);
            testClash.DefaultOffPeakExposureCount = exposureCount;
            CacheWrite(TestClashKey, testClash);
        }

        [Given("the clash has default peak exposure count of (.*)")]
        public void GivenTheClashHasDefaultPeakExposureCountOf(int exposureCount)
        {
            var testClash = CacheRead<Clash>(TestClashKey);
            testClash.DefaultPeakExposureCount = exposureCount;
            CacheWrite(TestClashKey, testClash);
        }

        [Given("a clash with no exposure count differences")]
        public void GivenAClashWithNoExposureCountDifferences()
        {
            var testClash = CacheRead<Clash>(TestClashKey);
            testClash.Differences.Clear();
            CacheWrite(TestClashKey, testClash);
        }

        [Given("a clash with these clash exposure differences")]
        public void GivenAClashWithTheseClashExposureDifferences(Table table)
        {
            var testClash = CacheRead<Clash>(TestClashKey);
            testClash.Differences.Clear();
            testClash.Differences.AddRange(table.CreateSet<ClashDifference>());
            CacheWrite(TestClashKey, testClash);
        }

        [When("I calculate the effective clash exposure count")]
        public void WhenICalculateTheEffectiveClashExposureCount()
        {
            (string start, string end) = CacheRead<(string start, string end)>(TestPeakKey);

            var fakeTenantSettings = _fixture
                .Build<(string peakStartTime, string peakEndTime)>()
                .With(p => p.peakStartTime, start)
                .With(p => p.peakEndTime, end)
                .Create();

            IClashExposureCountService clashExposureCountCalculator = ClashExposureCountService.Create(
                fakeTenantSettings
                );

            Clash testClash = CacheRead<Clash>(TestClashKey);
            Break testBreak = CacheRead<Break>(TestBreakKey);

            int result = clashExposureCountCalculator.Calculate(
                testClash.Differences,
                (testClash.DefaultPeakExposureCount, testClash.DefaultOffPeakExposureCount),
                (testBreak.ScheduledDate, testBreak.SalesArea)
                );

            CacheWrite(TestResultKey, result);
        }

        [Then("the result is the default off peak exposure count")]
        public void ThenTheResultIsTheDefaultOffPeakExposureCount()
        {
            Clash testClash = CacheRead<Clash>(TestClashKey);
            int result = CacheRead<int>(TestResultKey);

            _ = result.Should().Be(testClash.DefaultOffPeakExposureCount, null);
        }

        [Then("the result is the default peak exposure count")]
        public void ThenTheResultIsTheDefaultPeakExposureCount()
        {
            Clash testClash = CacheRead<Clash>(TestClashKey);
            int result = CacheRead<int>(TestResultKey);

            _ = result.Should().Be(testClash.DefaultPeakExposureCount, null);
        }

        [Then("the clash exposure count is (.*)")]
        public void ThenTheClashExposureCountIs(int exposureCount)
        {
            int result = CacheRead<int>(TestResultKey);
            _ = result.Should().Be(exposureCount, null);
        }
    }
}
