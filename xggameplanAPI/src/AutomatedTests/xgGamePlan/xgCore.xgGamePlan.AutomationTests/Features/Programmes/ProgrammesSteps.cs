using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ProgrammeCategory;
using xgCore.xgGamePlan.ApiEndPoints.Models.Programmes;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Extensions;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Programmes
{
    [Binding]
    public class ProgrammesSteps : BaseSteps<IProgrammesApi>
    {
        public ProgrammesSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many Programmes there are")]
        public async Task GivenIKnowHowManyProgrammesThereAre()
        {
            var programmes = await Api.GetAll().ConfigureAwait(false) ?? new List<Programme>();
            ScenarioContext.Set(programmes.Count(),"programmesCount");
        }

        [When(@"I add (.*) Programmes")]
        public async Task WhenIAddProgrammes(int count)
        {
            await CreateProgrammes(count).ConfigureAwait(false);
        }

        [When(@"I add (.*) Programmes for date range")]
        public async Task WhenIAddProgrammesForDateRange(int count)
        {
            var dateRange = ScenarioContext.Get<(DateTime from, DateTime to)>();
            await CreateProgrammes(count, dateRange.GetDateInRange()).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Programmes are returned")]
        public async Task ThenAdditionalProgrammesAreReturned(int count)
        {
            var allProgrammes = await Api.GetAll().ConfigureAwait(false);

            var existingProgramees = ScenarioContext.Get<int>("programmesCount");
            Assert.That(allProgrammes.Count(), Is.EqualTo(existingProgramees + count));
        }

        [Given(@"I have added (.*) Programmes")]
        public async Task GivenIHaveAddedProgrammes(int count)
        {
            await CreateProgrammes(count).ConfigureAwait(false);
        }

        [When(@"I delete all Programmes")]
        public async Task WhenIDeleteAllProgrammes()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Programmes are returned")]
        public async Task ThenNoProgrammesAreReturned()
        {
            var programmes = await Api.GetAll().ConfigureAwait(false);
            Assert.IsNull(programmes);
        }

        private async Task CreateProgrammes(int count, DateTime? date = null)
        {
            var salesArea = ScenarioContext.Get<SalesArea>();
            if (date == null)
            {
                date = Fixture.Create<DateTime>();
            }

            var programmeCategories = ScenarioContext.Get<List<ProgrammeCategoryHierarchy>>();
            var values = Fixture.Build<Programme>()
                .With(p => p.SalesArea,salesArea.Name)
                .With(p => p.Classification, programmeCategories.FirstOrDefault().Name)
                .With(p => p.ProgrammeCategories, programmeCategories.Select(c=>c.Name))
                .With(p => p.StartDateTime, date)
                .CreateMany(count);
            await Api.Create(values).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

    }
}
