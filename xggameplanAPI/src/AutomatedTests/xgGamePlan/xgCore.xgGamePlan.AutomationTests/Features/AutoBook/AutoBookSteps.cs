using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NodaTime;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook;
using xgCore.xgGamePlan.ApiEndPoints.Models.Runs;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.AutoBook
{
    [Binding]
    public class AutoBookSteps : BaseSteps<IAutoBookApi>
    {
        private const int _delayAutoBookSave = 3500;

        public AutoBookSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [When(@"I request AutoBook instance configurations")]
        public async Task WhenIRequestAutoBookInstanceConfigurations()
        {
            var autoBookInstanceConfigurations = await Api.GetAutoBookInstanceConfigurations()
                                                    .ConfigureAwait(false);
            ScenarioContext.Set(autoBookInstanceConfigurations);
        }

        [When(@"I request AutoBook settings")]
        public async Task WhenIRequestAutoBookSettings()
        {
            var autoBookSettings = await Api.GetAutoBookSettings()
                                                    .ConfigureAwait(false);
            ScenarioContext.Set(autoBookSettings);
        }

        [When(@"I update AutoBook settings with new values")]
        public async Task WhenIUpdateAutoBookSettingsWithNewValues()
        {
            var autoBookSettings = ScenarioContext.Get<AutoBookSettingsModel>();

            autoBookSettings.ProvisioningAPIURL = Fixture.Create<string>();
            autoBookSettings.AutoProvisioning = Fixture.Create<bool>();
            int minLifeTimeDays = Fixture.Create<int>();
            autoBookSettings.MinLifetime = Duration.FromDays(minLifeTimeDays);
            autoBookSettings.MaxLifetime = Duration.FromDays(minLifeTimeDays + 1);
            autoBookSettings.CreationTimeout = Duration.FromSeconds(Fixture.Create<int>());
            autoBookSettings.MinInstances = 0;
            var generator = Fixture.Create<Generator<int>>();
            autoBookSettings.MaxInstances = generator.Where(x => x > 0 && x < autoBookSettings.SystemMaxInstances).First(); ;
            autoBookSettings.ApplicationVersion = Fixture.Create<string>();
            autoBookSettings.BinariesVersion = Fixture.Create<string>();

            await Api.UpdateAutoBookSettings(autoBookSettings).ConfigureAwait(false);
            ScenarioContext.Set(autoBookSettings);
        }

        [Then(@"not-empty configuration collection is returned")]
        public void ThenNotEmptyConfigurationCollectionIsReturned()
        {
            var autoBookInstanceConfigurations = ScenarioContext.Get<IEnumerable<AutoBookInstanceConfiguration>>();
            Assert.IsNotEmpty(autoBookInstanceConfigurations);
        }

        [Then(@"updated AutoBook settings are returned")]
        public async Task ThenUpdatedAutoBookSettingsAreReturned()
        {
            var updatedAutoBookSettings = ScenarioContext.Get<AutoBookSettingsModel>();
            var autoBookSettings = await Api.GetAutoBookSettings().ConfigureAwait(false);
            Assert.AreEqual(updatedAutoBookSettings, autoBookSettings);
        }

        [Given(@"I know how many AutoBooks there are")]
        public async Task GivenIKnowHowManyAutoBooksThereAre()
        {
            int count = (await Api.GetAll().ConfigureAwait(false)).Count();
            ScenarioContext.Set(count, "previousCount");
        }

        [When(@"I add (.*) AutoBooks")]
        public async Task WhenIAddAutoBooks(int count)
        {
            await CreateAutoBooks(count).ConfigureAwait(false);
        }

        [Then(@"(.*) additional AutoBooks are returned")]
        public async Task ThenAdditionalAutoBooksAreReturned(int count)
        {
            int previousCount = ScenarioContext.Get<int>("previousCount");
            int newCount = (await Api.GetAll().ConfigureAwait(false)).Count();

            Assert.AreEqual(previousCount + count, newCount);
        }

        [Given(@"I have added new AutoBook")]
        public async Task GivenIHaveAddedNewAutoBook()
        {
            await CreateAndSaveOneAutoBook().ConfigureAwait(false);
        }

        [When(@"I get AutoBook by id")]
        public async Task WhenIGetAutoBookById()
        {
            string id = ScenarioContext.Get<AutoBookModel>("created").Id;
            ScenarioContext.Set(false, "deleted");

            AutoBookModel requestedModel = null;
            try
            {
                requestedModel = await Api.GetById(id).ConfigureAwait(false);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ScenarioContext.Set(true, "deleted");
            }
            ScenarioContext.Set(requestedModel, "requested");
        }

        [Then(@"created AutoBook is returned")]
        public void ThenCreatedAutoBookIsReturned()
        {
            var created = ScenarioContext.Get<AutoBookModel>("created");
            var requested = ScenarioContext.Get<AutoBookModel>("requested");

            Assert.AreEqual(created, requested);
        }

        [When(@"I delete AutoBook")]
        public async Task WhenIDeleteAutoBook()
        {
            string createdId = ScenarioContext.Get<AutoBookModel>("created").Id;
            await Api.Delete(createdId).ConfigureAwait(false);
            await Task.Delay(_delayAutoBookSave).ConfigureAwait(false);
        }

        [When(@"I get Audit trial for run's scenario")]
        public async Task WhenIGetAuditTrialForRunSScenario()
        {
            var scenarioId = ScenarioContext.Get<IEnumerable<Run>>().FirstOrDefault()?.Scenarios.FirstOrDefault()?.Id ?? Guid.Empty;
            string createdId = ScenarioContext.Get<AutoBookModel>("created").Id;

            string auditTrial = await Api.GetScenarioAuditTrail(createdId, scenarioId).ConfigureAwait(false);
            ScenarioContext.Set(auditTrial, "returnedString");
        }

        [Then(@"not empty string returned")]
        public void ThenNotEmptyStringReturned()
        {
            Assert.IsNotEmpty(ScenarioContext.Get<string>("returnedString"));
        }

        [When(@"I get logs for run's scenario")]
        public async Task WhenIGetLogsForRunSScenario()
        {
            var scenarioId = ScenarioContext.Get<IEnumerable<Run>>().FirstOrDefault()?.Scenarios.FirstOrDefault()?.Id ?? Guid.Empty;
            string createdId = ScenarioContext.Get<AutoBookModel>("created").Id;

            string logs = await Api.GetScenarioLogs(createdId, scenarioId).ConfigureAwait(false);
            ScenarioContext.Set(logs, "returnedString");
        }


        [Then(@"no AutoBook returned")]
        public void ThenNoAutoBookReturned()
        {
            Assert.That(ScenarioContext.Get<bool>("deleted"));
        }


        private async Task CreateAutoBooks(int count)
        {
            int instanceConfigurationId = (await Api.GetAutoBookInstanceConfigurations().ConfigureAwait(false))
                .FirstOrDefault()?.Id ?? default(int);

            for (int i = 0; i < count; i++)
            {
                await Api.Create(new CreateAutoBookModel()
                {
                    InstanceConfigurationId = instanceConfigurationId
                }).ConfigureAwait(false);
            }

            await Task.Delay(_delayAutoBookSave).ConfigureAwait(false);
        }

        private async Task CreateAndSaveOneAutoBook()
        {
            int instanceConfigurationId = (await Api.GetAutoBookInstanceConfigurations().ConfigureAwait(false))
                .FirstOrDefault()?.Id ?? default(int);

            var previousAutoBooks = await Api.GetAll().ConfigureAwait(false);

            await Api.Create(new CreateAutoBookModel()
            {
                InstanceConfigurationId = instanceConfigurationId
            }).ConfigureAwait(false);
            await Task.Delay(_delayAutoBookSave).ConfigureAwait(false);

            var newAutoBooks = await Api.GetAll().ConfigureAwait(false);
            var createdAutoBook = newAutoBooks.Except(previousAutoBooks, new AutoBookIdComparer()).FirstOrDefault();

            ScenarioContext.Set(createdAutoBook, "created");
        }
    }
}
