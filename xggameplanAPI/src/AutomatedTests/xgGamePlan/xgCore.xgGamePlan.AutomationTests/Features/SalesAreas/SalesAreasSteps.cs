using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NodaTime;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Demographics;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Universes
{
    [Binding]
    public class SalesAreasSteps : BaseSteps<ISalesAreasApi>
    {
        private readonly IDemographicsApi _demographicsApi;
        public SalesAreasSteps(ScenarioContext scenarioContext, IDemographicsApi demographicsApi) : base(scenarioContext)
        {
            _demographicsApi = demographicsApi;
        }

        //TODO: Refactor this as was created at start of development of framework
        [Given(@"I have a valid Sales Area")]
        public async Task GivenIHaveAValidSalesArea()
        {
            var demographic = Fixture.Build<Demographic>()
                .Without(p => p.Id)
                .With(p => p.Gameplan, true)
                .Create();
            var demographics = new List<Demographic>();
            demographics.Add(demographic);
            await _demographicsApi.Create(demographics).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);

            var salesArea = Fixture.Build<SalesArea>()
                .With(p => p.BaseDemographic1, demographic.ExternalRef)
                .With(p => p.BaseDemographic2, demographic.ExternalRef)
                .With(p => p.StartOffset, Duration.Zero)
                .With(p => p.DayDuration, Duration.FromHours(1))
                .Create();

            await Api.Create(salesArea).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(salesArea);
        }

        [Given(@"I know how many Sales Areas there are")]
        public async Task GivenIKnowHowManySalesAreasThereAre()
        {
            var salesAreas = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(salesAreas.Count(), "salesAreaCount");
        }

        [When(@"I add (.*) Sales Areas")]
        public async Task WhenIAddSalesAreas(int count)
        {
            var demographic = ScenarioContext.Get<Demographic>();
            await CreateSalesAreas(demographic, count).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Sales Areas are returned")]
        public async Task ThenAdditionalSalesAreasAreReturned(int additionalCount)
        {
            int previousCount = ScenarioContext.Get<int>("salesAreaCount");
            int actualCount = (await Api.GetAll().ConfigureAwait(false)).Count();

            Assert.AreEqual(actualCount, previousCount + additionalCount);
        }

        [Given(@"I have added (.*) Sales Area")]
        public async Task GivenIHaveAddedSalesArea(int count)
        {
            var demographic = ScenarioContext.Get<Demographic>();
            var salesAreas = await CreateSalesAreas(demographic, count).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(salesAreas);
        }

#region Unused Steps

        /*
        There is no API endpoint that accepts Custom ID, only by Raven Document Unique Id.
        But Raven Document Unique Id can't be retrieved directly from API.
        Because xggameplan.Models.SalesAreaModel doesn't contain the Raven Document Unique Id
        And it is no way to get Raven Document Unique Id from API
         So this Steps shouldn't be use until the API model or controller changed accordingly
        */

        [When(@"I request Sales Area by ID")]
        public async Task WhenIRequestSalesAreaByID()
        {
            var salesArea = (await Api.GetAll().ConfigureAwait(false)).FirstOrDefault();
            ScenarioContext.Set(salesArea);
        }

        [Then(@"Sales area with ID is returned")]
        public async Task ThenSalesAreaWithIDIsReturned()
        {
            int salesAreaId = ScenarioContext.Get<SalesArea>().Uid;
            var requestedSalesArea = await Api.GetById(salesAreaId).ConfigureAwait(false);

            Assert.AreEqual(salesAreaId, requestedSalesArea);
        }

        [When(@"I update Sales Area by ID")]
        public async Task WhenIUpdateSalesAreaByID()
        {
            var salesArea = (await Api.GetAll().ConfigureAwait(false)).FirstOrDefault();
            salesArea.Name = Fixture.Create<string>();
            ScenarioContext.Set(salesArea);
            await Api.Update(salesArea).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I remove my Sales Area by ID")]
        public async Task WhenIRemoveMySalesAreaByID()
        {
            var salesAreas = ScenarioContext.Get<IEnumerable<SalesArea>>();
            await Api.DeleteById(salesAreas.FirstOrDefault().Uid).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Sales Area with ID is returned")]
        public async Task ThenNoSalesAreaWithIDIsReturned()
        {
            var salesArea = ScenarioContext.Get<IEnumerable<SalesArea>>().FirstOrDefault();
            var response = await Api.GetByIdResponse(salesArea.Uid).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Wrong expected status code.");
        }

        [Then(@"updated Sales Area is returned")]
        public async Task ThenUpdatedSalesAreaIsReturned()
        {
            var salesArea = ScenarioContext.Get<SalesArea>();
            var updateSalesArea = await Api.GetById(salesArea.Uid).ConfigureAwait(false);

            Assert.AreEqual(salesArea, updateSalesArea);
        }

#endregion Unused Steps

        private async Task<IEnumerable<SalesArea>> CreateSalesAreas(Demographic demographic, int count)
        {
            var salesAreas = Fixture.Build<SalesArea>()
                .With(p => p.BaseDemographic1, demographic.ExternalRef)
                .With(p => p.BaseDemographic2, demographic.ExternalRef)
                .With(p => p.StartOffset, Duration.Zero)
                .With(p => p.DayDuration, Duration.FromHours(1))
                .CreateMany(count);

            //SalesArea API doesn't support parallel SaleArea creation.
            var res = new List<SalesArea>();

            foreach (var sa in salesAreas)
            {
                res.Add(await Api.Create(sa).ConfigureAwait(false));
            }

            return res;
        }
    }
}
