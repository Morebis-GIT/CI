using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Api;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.Clashes;
using xgCore.xgGamePlan.ApiEndPoints.Models.Products;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.ApiEndPoints.Utils;
using xgCore.xgGamePlan.AutomationTests.Extensions;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Clashes
{
    [Binding]
    public class ClashesSteps : BaseSteps<IClashesApi>
    {
        private readonly IProductsApi _productsApi;

        private const int ZERO = 0;

        public ClashesSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            var baseAddress = new Uri(ConfigReader.BaseAddress);
            var token = ConfigReader.AccessToken;
            _productsApi = ApiClientFactory.GetEndPoint<IProductsApi>(baseAddress, token);
        }

        [Given(@"I know how many Clashes there are")]
        public async Task GivenIKnowHowManyClashesThereAre()
        {
            var clashes = await Api.GetAll().ConfigureAwait(false) ?? new List<Clash>();
            ScenarioContext.Set(clashes.Count());
        }

        [Given(@"I have added (.*) Clashes")]
        public async Task GivenIHaveAddedClashes(int count)
        {
            await CreateClashes(count).ConfigureAwait(false);
        }

        [When(@"I add (.*) Clashes")]
        public async Task WhenIAddClashes(int count)
        {
            await CreateClashes(count).ConfigureAwait(false);
        }

        [Given(@"I know how many Clashes with description (.*) there are")]
        public async Task GivenIKnowHowManyClashesWithDescriptionThereAre(string description)
        {
            var allSpecifiedClashes = await Api.Search(new ClashSearchQueryModel
            {
                NameOrRef = description,
                Top = 0
            }).ConfigureAwait(false) ?? new SearchResultModel<Clash>();
            ScenarioContext.Set(allSpecifiedClashes.TotalCount);
        }

        [When(@"I add (.*) Clashes with the (.*) description")]
        public async Task WhenIAddClashesWithTheDescription(int count, string description)
        {
            await CreateClashesWithDescription(count, description).ConfigureAwait(false);
        }

        [When(@"I search Clashes by (.*) description")]
        public async Task WhenISearchClashesByDescription(string description)
        {
            var clashes = await Api.Search(new ClashSearchQueryModel() { NameOrRef = description }).ConfigureAwait(false);
            ScenarioContext.Set(clashes);
        }

        [When(@"I delete all Clashes")]
        public async Task WhenIDeleteClashes()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Clashes are returned")]
        public async void ThenNoClashesAreReturned()
        {
            var clashes = await Api.GetAll().ConfigureAwait(false);
            Assert.Zero(clashes.Count());
        }

        [Then(@"(.*) additional Clashes are returned")]
        public async Task ThenAdditionalClashesAreReturned(int count)
        {
            var allClashes = await Api.GetAll().ConfigureAwait(false) ?? new List<Clash>();

            var existingClashes = ScenarioContext.Get<int>();
            Assert.That(allClashes.Count(), Is.EqualTo(existingClashes + count));
        }

        [Then(@"(.*) additional Clashes are found")]
        public void ThenAdditionalClashesAreFound(int count)
        {
            var foundClashes = ScenarioContext.Get<SearchResultModel<Clash>>();
            var existingCount = ScenarioContext.Get<int>();
            Assert.That(foundClashes.TotalCount, Is.EqualTo(existingCount + count));
        }

        [Given(@"I have a valid Clash")]
        public async Task GivenIHaveAValidClash()
        {
            var clash = this.BuildCreateValidClashModelWithDifference();
            await Api.Create(new[] {clash}).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            var clashes = await Api.Search(new ClashSearchQueryModel() { NameOrRef = clash.Externalref }).ConfigureAwait(false);
            var existedClash = clashes.Items.FirstOrDefault();
            ScenarioContext.Set(existedClash);
        }

        [Given(@"I have added Clashes with external references '(.*)', '(.*)'")]
        public async Task GivenIHaveAddedClashes(string externalReference1, string externalReference2)
        {
            await CreateClashesForDelete(externalReference1, externalReference2).ConfigureAwait(false);
        }

        [Given(@"I have added Clash with external reference '(.*)'")]
        public async Task GivenIHaveAddedClash(string externalReference)
        {
            await CreateOneClashForDelete(externalReference).ConfigureAwait(false);
        }

        [When(@"I delete Clash by external reference '(.*)'")]
        public async Task WhenIDeleteClashByExternalId(string externalReference)
        {
            var response = await Api.DeleteByExternalReference(externalReference).ConfigureAwait(false);

            ScenarioContext.Add("DeleteByExternalResponse", response);
        }

        [Then(@"error response is received with message '(.*)'")]
        public void ThenErrorResponseIsReceivedWithMessage(string expectedMessage)
        {
            var response = ScenarioContext.Get<ApiResponse<string>>("DeleteByExternalResponse");
            var message = response.Error?.ToResponseErrorInfo()?.ErrorMessage;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.AreEqual(expectedMessage, message);
            });
        }

        [Given(@"I have added Product with Clash Code '(.*)'")]
        public async Task GivenIHaveAddedProducts(string clashCode)
        {
            await CreateProductForClashDelete(clashCode).ConfigureAwait(false);
        }

        [Then(@"ok response is received")]
        public void ThenOkResponseIsReceived()
        {
            var response = ScenarioContext.Get<ApiResponse<string>>("DeleteByExternalResponse");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Given(@"I have added Clash with difference for sales area '(.*)' and external reference '(.*)'")]
        public async Task GivenIHaveAddedClashWithDifferenceForSalesAreaAndExternalReference(string salesArea, string clashExternalReference)
        {
            //Delete previous clashes
            await Api.DeleteAll().ConfigureAwait(false);

            var createClashModel = BuildCreateClashModelWithDifference(clashExternalReference, salesArea);
            await CreateClashWithDifference(createClashModel).ConfigureAwait(false);

            ScenarioContext.Set(createClashModel, "CreateClashModel");
        }

        [When(@"I update Clash default peek and off-peak exposure count using external reference '(.*)' and Apply Globally set to '(.*)'")]
        public async Task WhenIUpdateClashDefaultPeekAndOffPeakExposureCountUsingExternalReferenceAndApplyGloballySetTo(string clashExternalReference, string applyGloballyWord)
        {
            if (!applyGloballyWord.TryParseToBoolean(out bool applyGlobally))
            {
                throw new ArgumentException($"Invalid {nameof(applyGloballyWord)} value provided");
            }

            var createClashModel = ScenarioContext.Get<CreateClash>("CreateClashModel");
            var updateClashModel = BuildClashUpdateModelFromCreateModel(createClashModel);

            var clashes = await Api.GetAll().ConfigureAwait(false);
            var targetClash = clashes.FirstOrDefault(c => c.Externalref == clashExternalReference);

            if (targetClash is null)
            {
                throw new ArgumentException(
                    $"Could not find Clash with external reference {clashExternalReference} which had to be created on previous step");
            }

            var result = await Api.Update(updateClashModel, targetClash.Uid, applyGlobally)
                .ConfigureAwait(false);

            ScenarioContext.Set(result, "UpdateClashResult");
        }

        [Then(@"There are no differences in returned Clash")]
        public void ThenThereAreNoDifferencesInReturnedClash()
        {
            var result = ScenarioContext.Get<ApiResponse<Clash>>("UpdateClashResult");

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.AreEqual(ZERO, result.Content?.Differences.Count);
            });
        }

        [Then(@"There is difference for Sales Area '(.*)' in returned Clash")]
        public void ThenThereIsDifferenceForSalesAreaInReturnedClash(string salesArea)
        {
            var result = ScenarioContext.Get<ApiResponse<Clash>>("UpdateClashResult");

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
                Assert.AreNotEqual(ZERO, result.Content?.Differences.Count);
                Assert.IsTrue(result.Content?.Differences.FirstOrDefault(c => c.SalesArea == salesArea) != null);
            });
        }

        private async Task CreateClashes(int count)
        {
            var newClashes = Fixture.Build<CreateClash>()
                .With(c => c.Externalref, () => string.Join(string.Empty, Guid.NewGuid().ToString().Skip(6).Take(6)))
                .With(c => c.ParentExternalidentifier, string.Empty)
                .With(c => c.Differences, new List<ClashDifference>())
                .CreateMany(count).ToList();
            await Api.Create(newClashes).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        private async Task CreateClashesForDelete(string externalReference1, string externalReference2)
        {
            var clashesHierarchy = new[]
            {
                Fixture.Build<CreateClash>()
                    .With(x => x.Externalref, externalReference1)
                    .With(c => c.ParentExternalidentifier, string.Empty)
                    .With(c => c.Differences, new List<ClashDifference>())
                    .With(x => x.DefaultOffPeakExposureCount, 100)
                    .With(x => x.DefaultPeakExposureCount, 200)
                .Create(),
                Fixture.Build<CreateClash>()
                    .With(x => x.Externalref, externalReference2)
                    .With(c => c.Differences, new List<ClashDifference>())
                    .With(x => x.ParentExternalidentifier, externalReference1)
                    .With(x => x.DefaultOffPeakExposureCount, 100)
                    .With(x => x.DefaultPeakExposureCount, 200)
                    .Create()
            };

            await Api.Create(clashesHierarchy).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        private async Task CreateClashesWithDescription(int count, string description)
        {
            var newClashes = Fixture.Build<CreateClash>()
                .With(c => c.Externalref, () => string.Join(string.Empty, Guid.NewGuid().ToString().Skip(6).Take(6)))
                .With(c => c.ParentExternalidentifier, string.Empty)
                .With(x => x.Description, description)
                .With(c => c.Differences, new List<ClashDifference>())
                .CreateMany(count).ToList();

            await Api.Create(newClashes).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        private async Task CreateOneClashForDelete(string externalReference)
        {
            var clash = Fixture.Build<CreateClash>()
                .With(c => c.ParentExternalidentifier, string.Empty)
                .With(x => x.Externalref, externalReference)
                .With(c => c.Differences, new List<ClashDifference>())
                .Create();

            await Api.Create(new [] {clash}).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        private async Task CreateProductForClashDelete(string clashCode)
        {
            var product = Fixture.Build<Product>()
                .With(x => x.ClashCode, clashCode)
                .With(x => x.EffectiveStartDate, DateTime.UtcNow.AddDays(-1))
                .With(x => x.EffectiveEndDate, DateTime.UtcNow.AddDays(1))
                .Create();

            await _productsApi.Create(new[] { product }).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        private CreateClash BuildCreateClashModelWithDifference(string externalReference, string salesArea)
        {
            var difference = Fixture.Build<ClashDifference>()
                .With(x => x.SalesArea, salesArea)
                .With(x => x.TimeAndDow, new ClashDifferenceTimeAndDow()
                {
                    StartTime = DateTime.UtcNow.Date.TimeOfDay,
                    EndTime = DateTime.UtcNow.Date.AddMilliseconds(-1).TimeOfDay,
                    DaysOfWeek = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"}
                })
                .With(x => x.StartDate, DateTime.UtcNow.Date)
                .With(x => x.EndDate, DateTime.UtcNow.Date.AddDays(1))
                .With(x => x.PeakExposureCount, 1)
                .With(x => x.OffPeakExposureCount, 1)
                .Create();

            return Fixture.Build<CreateClash>()
                .With(x => x.Externalref, externalReference)
                .With(x => x.ParentExternalidentifier, string.Empty)
                .With(x => x.Differences, new List<ClashDifference> { difference })
                .With(x => x.DefaultPeakExposureCount, 10)
                .With(x => x.DefaultOffPeakExposureCount, 10)
                .Create();
        }

        private CreateClash BuildCreateValidClashModelWithDifference()
        {
            var difference = Fixture.Build<ClashDifference>()
                .With(x => x.StartDate, DateTime.UtcNow.Date)
                .With(x => x.EndDate, DateTime.UtcNow.Date.AddDays(1))
                .With(x => x.TimeAndDow, new ClashDifferenceTimeAndDow()
                {
                    StartTime = DateTime.UtcNow.Date.AddHours(-1).TimeOfDay,
                    EndTime = DateTime.UtcNow.Date.TimeOfDay,
                    DaysOfWeek = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"}
                })
                .CreateMany(3).ToList();

            return Fixture.Build<CreateClash>()
                .With(x => x.Externalref, Guid.NewGuid().ToString().Substring(0,6))
                .With(x => x.ParentExternalidentifier, string.Empty)
                .With(x => x.Differences, difference)
                .Create();
        }

        private UpdateClashModel BuildClashUpdateModelFromCreateModel(CreateClash createClashModel)
        {
            return Fixture.Build<UpdateClashModel>()
                .With(x => x.Differences, createClashModel.Differences)
                .With(x => x.Description, createClashModel.Description)
                .With(x => x.ParentExternalidentifier, createClashModel.ParentExternalidentifier)
                .With(x => x.DefaultOffPeakExposureCount, 20)
                .With(x => x.DefaultPeakExposureCount, 20)
                .Create();
        }

        private async Task CreateClashWithDifference(CreateClash createClashModel)
        {
            await Api.Create(new[] { createClashModel }).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }
    }
}
