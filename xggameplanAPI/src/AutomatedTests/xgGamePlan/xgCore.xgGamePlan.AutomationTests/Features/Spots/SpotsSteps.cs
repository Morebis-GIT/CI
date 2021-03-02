using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NodaTime;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Breaks;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Models.Spots;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.ApiEndPoints.Utils;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Spots
{
    [Binding]
    public class SpotsSteps : BaseSteps<ISpotsApi>
    {
        private readonly IBreaksApi _breakApi;

        public SpotsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            var baseAddress = new Uri(ConfigReader.BaseAddress);
            string token = ConfigReader.AccessToken;
            _breakApi = ApiClientFactory.GetEndPoint<IBreaksApi>(baseAddress, token);

            ConfigureFixture();
        }

        private void ConfigureFixture()
        {
            Fixture.Customize<Spot>(composer => composer
                .Without(x => x.Product)
                .Without(x => x.MultipartSpot)
                .Without(x => x.ExternalBreakNo)
                .Without(x => x.EndDateTime)
                .Without(x => x.ActualPositioninBreak)
                .Without(x => x.BreakType)
                .Without(x => x.GroupCode)
                .Without(x => x.MultipartSpotPosition)
                .Without(x => x.RequestedPositioninBreak)
                .Without(x => x.BreakRequest)
                .Without(x => x.IndustryCode));
        }

        [Given(@"I have valid date value for Spot and Schedule")]
        public void GivenIHaveValidDateValueForSpotAndSchedule()
        {
            var validDate = DateTime.Now;
            ScenarioContext.Set(validDate);
        }

        [Given(@"I know how many Spots there are")]
        public async Task GivenIKnowHowManySpotsThereAre()
        {
            var spots = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(spots.Count());
        }

        [Given(@"I have added (.*) Spots")]
        public async Task GivenIHaveAddedSpots(int count)
        {
            var spots = CreateSpots(count);
            await SendSpots(spots).ConfigureAwait(false);
        }

        [Given(@"I know Spot external references")]
        public async Task GivenIKnowSpotExternalReferences()
        {
            var spots = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(spots.Select(s => s.ExternalSpotRef).ToList(), "ExternalSpotRef");
        }

        [Given(@"I know how many Spots for SalesArea and date there are")]
        public async Task GivenIKnowHowManySpotsForSalesAreaAndDateThereAre()
        {
            var spots = await Api.GetAll().ConfigureAwait(false);
            var date = ScenarioContext.Get<DateTime>();
            var salesArea = ScenarioContext.Get<SalesArea>();
            int count = spots.Count(x => x.StartDateTime == date.ToUniversalTime() && x.SalesArea == salesArea.Name);
            ScenarioContext.Set(count);
        }

        [Given(@"I have no Spots with provided external id and spot model")]
        public void GivenIHaveNoSpotsWithProvidedExternalId()
        {
            const string nonexistentSpotExternalId = "NONEXISTENT";
            var spot = new CreateUpdateSpot {ExternalSpotRef = nonexistentSpotExternalId};

            ScenarioContext.Set(nonexistentSpotExternalId, "nonexistentSpotExternalId");
            ScenarioContext.Set(spot, "spot");
        }

        [Given(@"I have valid spot data without break change")]
        public async Task GivenIHaveValidSpotDataWithoutBreakChange()
        {
            var initialSpot = CreateSpots(1).First();

            await Api.Create(new[] {initialSpot}).ConfigureAwait(false);

            initialSpot.Demographic = "TEST_DEMOGRAPHIC";

            ScenarioContext.Set(initialSpot, "spotPayload");
        }

        [Given(@"I have valid spot data with break change")]
        public async Task GivenIHaveValidSpotDataWithBreakChange()
        {
            var defaultSpotLength = TimeSpan.FromSeconds(15);
            var defaultBreakLength = TimeSpan.FromSeconds(30);

            string initialBreakReference = $"INITIAL_BREAK{Guid.NewGuid().ToString()}";
            var initialBreak = CreateBreak(initialBreakReference, defaultBreakLength);

            string targetBreakReference = $"TARGET_BREAK{Guid.NewGuid().ToString()}";
            var targetBreak = CreateBreak(targetBreakReference, defaultBreakLength);

            await _breakApi.Create(new[] {initialBreak, targetBreak}).ConfigureAwait(false);

            var initialSpot = CreateSpots(1, initialBreak).First();
            initialSpot.ExternalBreakNo = initialBreakReference;
            initialSpot.SpotLength = Duration.FromSeconds(defaultSpotLength.TotalSeconds);

            await Api.Create(new[] { initialSpot }).ConfigureAwait(false);

            initialSpot.MoveTo(targetBreak);

            ScenarioContext.Set(targetBreak, "updateSpotTargetBreak");
            ScenarioContext.Set(initialSpot, "spotPayload");
        }

        [Given(@"I have added (.*) Spots with break change")]
        public async Task GivenIHaveAddedSpotsWithBreakChange(int count)
        {
            var defaultSpotLength = TimeSpan.FromSeconds(15);
            var defaultBreakLength = TimeSpan.FromSeconds(30);

            string initialBreakReference = $"INITIAL_BREAK{Guid.NewGuid().ToString()}";
            var initialBreak = CreateBreak(initialBreakReference, defaultBreakLength);

            await _breakApi.Create(new[] {initialBreak}).ConfigureAwait(false);

            var spots = CreateSpots(count, initialBreak);
            foreach (var spot in spots)
            {
                spot.ExternalBreakNo = initialBreakReference;
                spot.SpotLength = Duration.FromSeconds(defaultSpotLength.TotalSeconds);
            }

            await SendSpots(spots).ConfigureAwait(false);

            ScenarioContext.Set(initialBreak, "spotInitialBreak");
        }

        [When(@"I add (.*) Spots")]
        public async Task WhenIAddSpots(int count)
        {
            var spots = CreateSpots(count);
            await SendSpots(spots).ConfigureAwait(false);
        }

        [When(@"I search Spots by SalesArea and date")]
        public async Task WhenISearchSpotsBySalesAreaAndDate()
        {
            var date = ScenarioContext.Get<DateTime>();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var query = new SearchSpotsQuery()
            {
                DateFrom = date,
                DateTo = date.AddSeconds(1),
                SalesArea = salesArea.Name
            };
            var spots = await Api.Search(query).ConfigureAwait(false);
            ScenarioContext.Set(spots);
        }

        [When(@"I search Spots with break and programme details by SalesArea and date")]
        public async Task WhenISearchSpotsWithBreakAndProgrammeDetailsBySalesAreaAndDate()
        {
            var date = ScenarioContext.Get<DateTime>();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var query = new SearchSpotsQuery()
            {
                DateFrom = date,
                DateTo = date.AddSeconds(1),
                SalesArea = salesArea.Name
            };
            var spots = await Api.SearchWithBreakAndProgrammeInfo(query).ConfigureAwait(false);
            ScenarioContext.Set(spots);
        }

        [When(@"I delete Spots by date range and SalesArea")]
        public async Task WhenIDeleteSpotsByDateRangeAndSalesArea()
        {
            var spotInitialBreak = ScenarioContext.Get<Break>("spotInitialBreak");
            var dateFrom = spotInitialBreak.ScheduledDate.AddDays(-1);
            var dateTo = spotInitialBreak.ScheduledDate.AddDays(1);
            var salesAreaNames = new List<string> {ScenarioContext.Get<SalesArea>().Name};
            await Api.Delete(dateFrom, dateTo, salesAreaNames).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I delete Spots by external references")]
        public async Task WhenIDeleteSpotsByExternalReferences()
        {
            var externalSpotRefs = ScenarioContext.Get<List<string>>("ExternalSpotRef");
            await Api.Delete(externalSpotRefs).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I delete all Spots")]
        public async Task WhenIDeleteAllSpots()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I upsert spot with provided external id")]
        public async Task WhenIUpsertSpotWithProvidedExternalId()
        {
            string nonexistentSpotExternalId = ScenarioContext.Get<string>("nonexistentSpotExternalId");
            var spot = ScenarioContext.Get<CreateUpdateSpot>("spot");

            var response = await Api.Put(nonexistentSpotExternalId, spot).ConfigureAwait(false);

            ScenarioContext.Set(response, "spotUpdateResult");
        }

        [When(@"I upsert spot")]
        public async Task WhenIUpsertSpot()
        {
            var domainSpotPayload = ScenarioContext.Get<Spot>("spotPayload");

            var model = BuildCreateSpotModel(domainSpotPayload);

            var updateResult = await Api.Put(domainSpotPayload.ExternalSpotRef, model).ConfigureAwait(false);

            ScenarioContext.Set(domainSpotPayload, "expectedSpot");
            ScenarioContext.Set(updateResult, "spotUpdateResult");
        }

        [Then(@"(.*) additional Spots are returned")]
        public async Task ThenAdditionalSpotsAreReturned(int count)
        {
            var allSpots = await Api.GetAll().ConfigureAwait(false);

            int knownCount = ScenarioContext.Get<int>();
            Assert.AreEqual(allSpots.Count(), knownCount + count);
        }

        [Then(@"(.*) additional Spots are found")]
        public void ThenAdditionalSpotsAreFound(int count)
        {
            var result = ScenarioContext.Get<IEnumerable<SpotModel>>();
            int knownCount = ScenarioContext.Get<int>();
            Assert.AreEqual(result.Count(), knownCount + count);
        }

        [Then(@"(.*) additional Spots with break and programme are found")]
        public void ThenAdditionalSpotsWithBreakAndProgrammeAreFound(int count)
        {
            var result = ScenarioContext.Get<IEnumerable<SpotWithBreakAndProgrammeInfo>>();
            int knownCount = ScenarioContext.Get<int>();
            Assert.AreEqual(result.Count(), knownCount + count);
        }

        [Then(@"no Spots within date range are returned")]
        public async Task ThenNoSpotsWithinDateRangeAreReturned()
        {
            var dateFrom = ScenarioContext.Get<DateTime>();
            var dateTo = dateFrom.Date.Add(DateTime.MaxValue.TimeOfDay);
            string salesAreaName = ScenarioContext.Get<SalesArea>().Name;
            var allSpots = await Api.GetAll().ConfigureAwait(false);

            Assert.IsEmpty(allSpots.Where(x =>
                x.StartDateTime >= dateFrom.Date && x.StartDateTime <= dateTo && x.SalesArea == salesAreaName));
        }

        [Then(@"no Spots are returned")]
        public async Task ThenNoSpotsAreReturned()
        {
            var allSpots = await Api.GetAll().ConfigureAwait(false);
            Assert.IsEmpty(allSpots);
        }

        [Then(@"I receive not found response")]
        public void ThenIReceiveNotFoundResponse()
        {
            var response = ScenarioContext.Get<ApiResponse<Spot>>("spotUpdateResult");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Then(@"it is successfully updated without break change")]
        public void ThenItIsSuccessfullyUpdatedWithoutBreakChange()
        {
            var updateResult = ScenarioContext.Get<ApiResponse<Spot>>("spotUpdateResult");
            var expectedSpot = ScenarioContext.Get<Spot>("expectedSpot");

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, updateResult.StatusCode);
                Assert.AreEqual(expectedSpot, updateResult.Content);
            });
        }

        [Then(@"it is successfully updated with break change")]
        public void ThenItIsSuccessfullyUpdatedWithBreakChange()
        {
            var updateResult = ScenarioContext.Get<ApiResponse<Spot>>("spotUpdateResult");
            var expectedSpot = ScenarioContext.Get<Spot>("expectedSpot");

            var targetBreak = ScenarioContext.Get<Break>("updateSpotTargetBreak");
            expectedSpot = MoveDateTimeRangeToBreakScheduledDate(expectedSpot, targetBreak);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, updateResult.StatusCode);
                Assert.AreEqual(expectedSpot, updateResult.Content);
            });
        }

        private IEnumerable<Spot> CreateSpots(int count, Break spotBreak = null)
        {
            var date = ScenarioContext.Get<DateTime>();
            var salesArea = ScenarioContext.Get<SalesArea>();

            var spots = Fixture.CreateMany<Spot>(count).ToList();

            if (spotBreak != null)
            {
                spots.ForEach(x => {
                    x.ExternalBreakNo = spotBreak.ExternalBreakRef;
                    x.SalesArea = spotBreak.SalesArea;
                    x.StartDateTime = spotBreak.ScheduledDate;
                });
            }
            else
            {
                spots.ForEach(x => {
                    x.SalesArea = salesArea.Name;
                    x.StartDateTime = date.ToUniversalTime();
                    x.EndDateTime = date.AddSeconds(1).ToUniversalTime();
                });                    
            }

            return spots;
        }

        private async Task SendSpots(IEnumerable<Spot> spots)
        {
            await Api.Create(spots).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        private static CreateUpdateSpot BuildCreateSpotModel(Spot spot)
        {
            return new CreateUpdateSpot
            {
                ExternalCampaignNumber = spot.ExternalCampaignNumber,
                SalesArea = spot.SalesArea,
                GroupCode = spot.GroupCode,
                ExternalSpotRef = spot.ExternalSpotRef,
                StartDateTime = spot.StartDateTime,
                EndDateTime = spot.EndDateTime,
                SpotLength = spot.SpotLength,
                BreakType = spot.BreakType,
                Product = spot.Product,
                Demographic = spot.Demographic,
                ClientPicked = spot.ClientPicked,
                MultipartSpot = spot.MultipartSpot,
                MultipartSpotPosition = spot.MultipartSpotPosition,
                MultipartSpotRef = spot.MultipartSpotRef,
                RequestedPositionInBreak = spot.RequestedPositioninBreak,
                ActualPositionInBreak = spot.ActualPositioninBreak,
                BreakRequest = spot.BreakRequest,
                ExternalBreakNo = spot.ExternalBreakNo,
                Sponsored = spot.Sponsored,
                Preemptable = spot.Preemptable,
                PreemptLevel = spot.Preemptlevel,
                IndustryCode = spot.IndustryCode,
                ClearanceCode = spot.ClearanceCode
            };
        }

        private Break CreateBreak(string externalReference, TimeSpan duration)
        {
            string salesAreaName = ScenarioContext.Get<SalesArea>().Name;
            const string defaultBreakTypeName = "BASE";
            return Fixture.Build<Break>()
                .With(x => x.ExternalBreakRef, externalReference)
                .With(x => x.Duration, duration)
                .With(x => x.SalesArea, salesAreaName)
                .With(x => x.BreakType, defaultBreakTypeName)
                .Create();
        }

        private static Spot MoveDateTimeRangeToBreakScheduledDate(Spot spot, Break targetBreak)
        {
            if (spot.EndDateTime != default(DateTime))
            {
                var diff = spot.EndDateTime - spot.StartDateTime;
                spot.EndDateTime = targetBreak.ScheduledDate + diff;
            }

            spot.StartDateTime = targetBreak.ScheduledDate;

            return spot;
        }
    }
}
