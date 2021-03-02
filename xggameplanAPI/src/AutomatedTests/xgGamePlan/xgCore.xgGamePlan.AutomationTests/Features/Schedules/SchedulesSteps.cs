using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Breaks;
using xgCore.xgGamePlan.ApiEndPoints.Models.ProgrammeCategory;
using xgCore.xgGamePlan.ApiEndPoints.Models.Programmes;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Schedules
{
    [Binding]
    public class SchedulesSteps : BaseSteps<ISchedulesApi>
    {
        private readonly IProgrammesApi _programmesApi;

        public SchedulesSteps(ScenarioContext scenarioContext, IProgrammesApi programmesApi) : base(scenarioContext)
        {
            _programmesApi = programmesApi;
        }

        [Given(@"I have added (.*) Schedules")]
        public async Task GivenIHaveAddedSchedules(int count)
        {
            await CreateSchedules(count).ConfigureAwait(false);
        }

        [When(@"I add (.*) Schedules")]
        public async Task WhenIAddSchedules(int count)
        {
            var programmes = ScenarioContext.Get<IEnumerable<Programme>>();
            await CreateSchedules(programmes.Count() + count).ConfigureAwait(false);
        }

        [Given(@"I know how many Programmes within the Schedules for date range")]
        public async Task GivenIKnowHowManyProgrammesWithinTheSchedulesForDateRange()
        {
            var programmes = await GetProgrammes().ConfigureAwait(false);
            ScenarioContext.Set(programmes == null ? 0 : programmes.Count());
        }

        [Given(@"I know how many Breaks within the Schedules for date range")]
        public async Task GivenIKnowHowManyBreaksWithinTheSchedulesForDateRange()
        {
            var breaks = await GetBreaks().ConfigureAwait(false);
            ScenarioContext.Set(breaks == null ? 0 : breaks.Count());
        }

        [When(@"I delete all Schedules")]
        public async Task WhenIDeleteAllSchedules()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Breaks within the Schedules are returned")]
        public async Task ThenAdditionalBreaksWithinTheSchedulesAreReturned(int count)
        {
            var breaks = await GetBreaks().ConfigureAwait(false);

            var existingBreaks = ScenarioContext.Get<int>();
            Assert.That(breaks.Count(), Is.EqualTo(existingBreaks + count));
        }

        [Then(@"(.*) additional Programmes within the Schedules are returned")]
        public async Task ThenAdditionalProgrammesWithinTheSchedulesAreReturned(int count)
        {
            var programmes = await GetProgrammes().ConfigureAwait(false);

            var existingProgrammes = ScenarioContext.Get<int>();
            Assert.That(programmes.Count(), Is.EqualTo(existingProgrammes + count));
        }

        [Then(@"no Schedules are returned")]
        public async Task ThenNoSchedulesAreReturned()
        {
            var schedules = await Api.GetAll().ConfigureAwait(false);
            Assert.IsNull(schedules);
        }

        private async Task<IEnumerable<Programme>> GetProgrammes()
        {
            var dateRange = ScenarioContext.Get<(DateTime from, DateTime to)>();
            var salesArea = ScenarioContext.Get<SalesArea>();
            return await Api.GetProgrammes(dateRange.from, dateRange.to,
                                new string[] { salesArea.Name.ToUpper(CultureInfo.InvariantCulture) })
                          .ConfigureAwait(false);
        }

        private async Task<IEnumerable<Break>> GetBreaks()
        {
            var dateRange = ScenarioContext.Get<(DateTime from, DateTime to)>();
            var salesArea = ScenarioContext.Get<SalesArea>();
            return await Api.GetBreaks(dateRange.from, dateRange.to,
                                new string[] { salesArea.Name.ToUpper(CultureInfo.InvariantCulture) })
                          .ConfigureAwait(false);
        }

        private async Task CreateSchedules(int count)
        {
            var date = ScenarioContext.Get<DateTime>();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var programmeCategories = ScenarioContext.Get<List<ProgrammeCategoryHierarchy>>();
            var programmes = Fixture.Build<Programme>()
                .With(p => p.SalesArea, salesArea.Name)
                .With(p => p.Classification, programmeCategories.FirstOrDefault().Name)
                .With(p => p.ProgrammeCategories, programmeCategories.Select(pc=>pc.Name))
                .With(p => p.StartDateTime, date.ToUniversalTime())
                .CreateMany(count);
            await _programmesApi.Create(programmes).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(programmes);
        }
    }
}
