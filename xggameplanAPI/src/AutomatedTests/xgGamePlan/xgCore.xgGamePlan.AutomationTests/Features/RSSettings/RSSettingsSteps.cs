using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.RSSettings;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.RSSettings
{
    [Binding]
    public class RSSettingsSteps : BaseSteps<IRSSettingsApi>
    {
        private readonly IDemographicsApi _demographicsApi;
        public RSSettingsSteps(ScenarioContext context, IDemographicsApi demographicsApi) : base(context)
        {
            _demographicsApi = demographicsApi;
        }

        [Given(@"I have created (.*) RSSettings with valid salesArea")]
        public async Task GivenIHaveCreatedRSSettingsWithValidSalesArea(int count)
        {
            await CreateRSSettings(count).ConfigureAwait(false);
        }

        [When(@"I request RSSettings by salesArea")]
        public async Task WhenIRequestRSSettingsBySalesArea()
        {
            await SetCountOfRSSettingsToScenarioContext().ConfigureAwait(false);
        }

        [Then(@"(.*) RSSettings are returned")]
        public void ThenRSSetingsAreReturned(int count)
        {
            int countOfRSSettings = ScenarioContext.Get<int>("countOfRSSettings");
            Assert.That(countOfRSSettings == count);
        }

        [Given(@"I know how many RSSettings with valid salesArea there are")]
        public async Task GivenIKnowHowManyRSSettingsWithValidSalesAreaThereAre()
        {
            await SetCountOfRSSettingsToScenarioContext().ConfigureAwait(false);
        }

        [When(@"I add (.*) RSSettings")]
        public async Task WhenIAddRSSettings(int count)
        {
            await CreateRSSettings(count).ConfigureAwait(false);
        }

        [Then(@"(.*) additional RSSettings are returned")]
        public async Task ThenAdditionalRSSetingsAreReturned(int count)
        {
            int prevCount = ScenarioContext.Get<int>("countOfRSSettings");
            int currentCount = await GetCount().ConfigureAwait(false);
            Assert.That(prevCount + count == currentCount);
        }

        [When(@"I update RSSetting with mode (.*)")]
        public async Task WhenIUpdateRSSettingWithMode(string mode)
        {
            mode = mode.Trim().Replace("\'", string.Empty);
            int updateMode = int.Parse(mode, NumberStyles.Integer, CultureInfo.InvariantCulture);

            var command = (RSSettingsModel)ScenarioContext.Get<RSSettingsModel>("createdRSSettings").Clone();
            var salesArea = ScenarioContext.Get<SalesArea>();
            
            var demographic = await _demographicsApi.GetByExternalRef(salesArea.BaseDemographic1).ConfigureAwait(false);

            command.DemographicsSettings = new List<RSDemographicSettingsModel>()
            {
                new RSDemographicSettingsModel()
                {
                    DemographicId = demographic.ExternalRef,
                    DeliverySettingsList = new List<RSDeliverySettingsModel>()
                    {
                        new RSDeliverySettingsModel()
                        {
                            DaysToCampaignEnd = 30,
                            LowerLimitOfOverDelivery = 90,
                            UpperLimitOfOverDelivery = 100
                        }
                    }
                }
            };

            foreach (var settings in command.DefaultDeliverySettingsList)
            {
                settings.DaysToCampaignEnd++;
                settings.LowerLimitOfOverDelivery++;
                settings.UpperLimitOfOverDelivery++;
            }

            var updatedRSSettings = await Api.Update(command, updateMode).ConfigureAwait(false);
            ScenarioContext.Set(updatedRSSettings, "updatedRSSettings");
        }

        [Then(@"updated RSSetting returned")]
        public void ThenUpdatedRSSettingReturned()
        {
            var createdRSSettings = ScenarioContext.Get<RSSettingsModel>("createdRSSettings");
            var updatedRSSettings = ScenarioContext.Get<RSSettingsModel>("updatedRSSettings");

            Assert.AreEqual(createdRSSettings.Id, updatedRSSettings.Id);
            Assert.AreNotEqual(createdRSSettings, updatedRSSettings);
        }

        [When(@"I delete all RSSettings by salesArea")]
        public async Task WhenIDeleteAllRSSettingsBySalesArea()
        {
            string salesAreaName = ScenarioContext.Get<SalesArea>().Name;
            await Api.Delete(salesAreaName).ConfigureAwait(false);
            await SetCountOfRSSettingsToScenarioContext().ConfigureAwait(false);
        }

        [When(@"I compare RSSettings with mode (.*)")]
        public async Task WhenICompareRSSettingsWithMode(string mode)
        {
            mode = mode.Trim().Replace("\'", string.Empty);
            int createMode = int.Parse(mode, NumberStyles.Integer, CultureInfo.InvariantCulture);
            await Api.Compare(createMode).ConfigureAwait(false);
        }


        private IEnumerable<RSSettingsModel> CreateRSSettingsFixtures(int count, string salesAreaName)
        {
            var rsDeliverySettings = new List<RSDeliverySettingsModel>()
            {
                new RSDeliverySettingsModel()
                {
                    DaysToCampaignEnd = 30,
                    LowerLimitOfOverDelivery = 90,
                    UpperLimitOfOverDelivery = 100
                }
            };

            return Fixture.Build<RSSettingsModel>()
                .Without(p => p.Id)
                .With(p => p.SalesArea, salesAreaName)
                .With(p => p.DefaultDeliverySettingsList, rsDeliverySettings)
                .With(p => p.DemographicsSettings, new List<RSDemographicSettingsModel>())
                .CreateMany(count);
        }

        private async Task SetCountOfRSSettingsToScenarioContext()
        {
            int countOfRSSettings = await GetCount().ConfigureAwait(false);
            ScenarioContext.Set(countOfRSSettings, "countOfRSSettings");
        }

        private async Task<int> GetCount()
        {
            string salesAreaName = ScenarioContext.Get<SalesArea>().Name;

            try
            {
                await Api.GetBySalesArea(salesAreaName).ConfigureAwait(false);
                return 1;
            }
            catch (ApiException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return 0;
            }
        }

        private async Task CreateRSSettings(int count)
        {
            string salesAreaName = ScenarioContext.Get<SalesArea>().Name;
            var rsSettingsList = CreateRSSettingsFixtures(count, salesAreaName);

            foreach (var rsSettings in rsSettingsList)
            {
                if (count == 1)
                {
                    var createdRSSettings = await Api.Create(rsSettings).ConfigureAwait(false);
                    ScenarioContext.Set(createdRSSettings, "createdRSSettings");
                    break;
                }

                await Api.Create(rsSettings).ConfigureAwait(false);
            }
        }
    }
}
