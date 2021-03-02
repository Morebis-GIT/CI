using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using NodaTime;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns;
using xgCore.xgGamePlan.ApiEndPoints.Models.Demographics;
using xgCore.xgGamePlan.ApiEndPoints.Models.Products;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace xgCore.xgGamePlan.AutomationTests.Features.Campaigns
{
    [Binding]
    public class CampaignsSteps : BaseSteps<ICampaignsApi>
    {
        private readonly IProductsApi _productsApi;

        private readonly FieldInfo[] _rightSizerFields = typeof(IncludeRightSizer).GetFields(BindingFlags.Public | BindingFlags.Static);
        private readonly Random _randomizer = new Random(DateTime.Now.Millisecond);

        public CampaignsSteps(ScenarioContext scenarioContext, IProductsApi productsApi) : base(scenarioContext)
        {
            _productsApi = productsApi;
            ConfigureFixture();
        }

        private void ConfigureFixture()
        {
            Fixture.Customize<Duration>(x => x.FromSeed(s => Duration.FromMinutes(1)));

            Fixture.Customize<Multipart>(composer => composer
                    .With(x => x.CurrentPercentageSplit, 5.5m)
                    .With(x => x.DesiredPercentageSplit, 5.5m));

            Fixture.Customize<Timeslice>(composer => composer
                    .With(p => p.FromTime, "12:00")
                    .With(p => p.ToTime, "13:00")
                    .With(p => p.DowPattern, new List<string> { "Sun" }));

            Fixture.Customize<DayPart>(composer => composer
                    .With(x => x.CurrentPercentageSplit, 5.5m)
                    .With(x => x.DesiredPercentageSplit, 5.5m)
                    .Without(x => x.Lengths)
                    .Do(x =>
                    {
                        x.Lengths = new List<DayPartLength> { Fixture.Create<DayPartLength>() };
                    }));

            Fixture.Customize<DayPartLength>(composer => composer
                    .With(x => x.Length, Duration.FromSeconds(15))
                    .With(x => x.CurrentPercentageSplit, 5.5m)
                    .With(x => x.DesiredPercentageSplit, 5.5m)
                    .With(x => x.MultipartNumber, 0));

            Fixture.Customize<LengthInformation>(composer => composer
                   .With(x => x.CurrentPercentageSplit, 5.5m)
                   .With(x => x.DesiredPercentageSplit, 5.5m));

            Fixture.Customize<SalesAreaCampaignTarget>(composer => composer
                    .Without(x => x.Multiparts)
                    .Without(x => x.CampaignTargets)
                    .Do(x =>
                    {
                        x.Multiparts = Fixture.CreateMany<Multipart>(1).ToList();
                        x.CampaignTargets = Fixture.CreateMany<CampaignTarget>(1).ToList();
                    }));

            Fixture.Customize<TimeRestriction>(composer => composer
                    .With(x => x.IsIncludeOrExclude, "I")
                    .With(x => x.DowPattern, new List<string> { "Sun" })
                    .Without(x => x.StartDateTime).Without(x => x.EndDateTime)
                    .Do(x =>
                    {
                        x.StartDateTime = new DateTime(Fixture.Create<DateTime>().Ticks, DateTimeKind.Utc);
                        x.EndDateTime = x.StartDateTime + Fixture.Create<TimeSpan>();
                    }));

            Fixture.Customize<ProgrammeRestriction>(composer => composer
                    .With(x => x.IsCategoryOrProgramme, "P")
                    .With(x => x.IsIncludeOrExclude, "I"));

            Fixture.Customize<CampaignTarget>(composer => composer
                    .Without(x => x.StrikeWeights)
                    .Without(x => x.StrikeWeights)
                    .Do(x =>
                    {
                        x.StrikeWeights = new List<StrikeWeight> { Fixture.Create<StrikeWeight>() };
                    }));

            Fixture.Customize<StrikeWeight>(composer => composer
                    .With(x => x.DesiredPercentageSplit, 5.5m)
                    .With(x => x.CurrentPercentageSplit, 5.5m)
                    .Without(x => x.StartDate)
                    .Without(x => x.EndDate)
                    .Without(x => x.DayParts)
                    .Without(x => x.Lengths)
                    .Do(x =>
                    {
                        x.StartDate = new DateTime(Fixture.Create<DateTime>().Ticks, DateTimeKind.Utc);
                        x.EndDate = x.StartDate + Fixture.Create<TimeSpan>();
                        x.DayParts = new List<DayPart> { Fixture.Create<DayPart>() };
                        x.Lengths = new List<LengthInformation> { Fixture.Create<LengthInformation>() };
                    }));

            Fixture.Customize<CreateCampaign>(composer => composer
                .With(x => x.TargetRatings, 5.5m)
                .With(x => x.ActualRatings, 5.5m)
                .With(c => c.CampaignGroup, () => $"CampaignGroup_{_randomizer.Next(1000000)}")
                .With(c => c.BusinessType, "Dynamic")
                .With(c => c.DeliveryType, "Rating")
                .With(c => c.IncludeOptimisation, true)
                .With(c => c.CampaignPassPriority, () => _randomizer.Next(0, 4))
                .With(c => c.IncludeRightSizer,
                    () => _rightSizerFields[_randomizer.Next(0, 3)]
                        .GetCustomAttribute<DescriptionAttribute>().Description)
                .Without(x => x.SalesAreaCampaignTarget)
                .Without(x => x.CampaignGroup)
                .Without(x => x.IncludeRightSizer)
                .Without(x => x.StartDateTime)
                .Without(x => x.EndDateTime)
                .Without(x => x.Status)
                .Do(x =>
                {
                    x.SalesAreaCampaignTarget = new List<SalesAreaCampaignTarget> { Fixture.Create<SalesAreaCampaignTarget>() };
                    x.StartDateTime = new DateTime(Fixture.Create<DateTime>().Ticks, DateTimeKind.Utc);
                    x.EndDateTime = x.StartDateTime.AddYears(1);
                    x.Status = "A";
                }));
        }

        [Given(@"I have added (.*) Campaigns")]
        public async Task GivenIHaveAddedCampaigns(int count)
        {
            var newCampaings = Fixture.CreateMany<CreateCampaign>(count).ToList();
            await CreateCampaigns(newCampaings).ConfigureAwait(false);
        }

        [Given(@"I know Campaign id")]
        public async Task GivenIKnowCampaignId()
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(campaigns.FirstOrDefault().Uid);
        }

        [Given(@"I know Campaign external id")]
        public async Task GivenIKnowCampaignExternalId()
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(campaigns.FirstOrDefault());
        }

        [Given(@"I know how many Campaigns there are")]
        public async Task GivenIKnowHowManyCampaignsThereAre()
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(campaigns.Count());
        }

        [Given(@"I know Campaign externalRef")]
        public async Task GivenIKnowCampaignExternalRef()
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);
            var externalRef = campaigns.FirstOrDefault()?.ExternalId;
            ScenarioContext.Set(externalRef);
        }

        [Given(@"I know how many Campaigns in group (.*)")]
        public async Task GivenIKnowHowManyCampaignsInGroup(string group)
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);
            var count = campaigns.Where(x => x.CampaignGroup == group).Count();
            ScenarioContext.Set(count);
        }

        [Given(@"I know how many active Campaigns there are")]
        public async Task GivenIKnowHowManyActiveCampaignsThereAre()
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);
            var count = campaigns.Where(x => x.Status == "A").Count();
            ScenarioContext.Set(count, "existingActiveCampaigns");
        }

        [When(@"I add (.*) Campaigns")]
        public async Task WhenIAddCampaigns(int count)
        {
            var newCampaings = Fixture.CreateMany<CreateCampaign>(count).ToList();
            await CreateCampaigns(newCampaings).ConfigureAwait(false);
        }

        [When(@"I add (.*) Campaigns in group (.*)")]
        public async Task WhenIAddCampaignsInGroup(int count, string group)
        {
            var newCampaings = Fixture.CreateMany<CreateCampaign>(count).ToList();
            newCampaings.ForEach(x => x.CampaignGroup = group);
            await CreateCampaigns(newCampaings).ConfigureAwait(false);
        }

        [When(@"I add (.*) active Campaigns")]
        public async Task WhenIAddActiveCampaigns(int count)
        {
            var newCampaings = Fixture.CreateMany<CreateCampaign>(count).ToList();
            newCampaings.ForEach(x => x.Status = "A");
            await CreateCampaigns(newCampaings).ConfigureAwait(false);
        }

        [When(@"I search active Campaigns")]
        public async Task WhenISearchActiveCampaigns()
        {
            var searchModel = new CampaignSearchQueryModel { Status = CampaignStatus.Active };
            var activeCampaigns = await Api.Search(searchModel).ConfigureAwait(false);
            ScenarioContext.Set(activeCampaigns.TotalCount, "totalActiveCampaigns");
        }

        [When(@"I get Campaign by id")]
        public async Task WhenIGetCampaignById()
        {
            var campaignId = ScenarioContext.Get<Guid>();
            var campaign = await Api.GetById(campaignId).ConfigureAwait(false);
            ScenarioContext.Set(campaign);
        }

        [When(@"I delete all Campaigns")]
        public async Task WhenIDeleteAllCampaigns()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I get Campaign by externalRef")]
        public async Task WhenIGetCampaignByExternalRef()
        {
            var externalRef = ScenarioContext.Get<string>();
            var campaign = await Api.GetByExternalRef(externalRef).ConfigureAwait(false);
            ScenarioContext.Set(campaign);
        }

        [When(@"I get Campaigns in group (.*)")]
        public async Task WhenIGetCampaignsInGroup(string group)
        {
            var campaigns = await Api.GetByGroup(group).ConfigureAwait(false);
            ScenarioContext.Set(campaigns);
        }

        [Then(@"Campaign is returned")]
        public void ThenCampaignIsReturned()
        {
            var campaign = ScenarioContext.Get<Campaign>();
            Assert.IsNotNull(campaign);
        }

        [Then(@"(.*) additional Campaigns in group are returned")]
        public void CampaignsInGroupAreReturned(int count)
        {
            var campaigns = ScenarioContext.Get<List<Campaign>>();

            var existingCampaigns = ScenarioContext.Get<int>();
            Assert.That(campaigns.Count, Is.EqualTo(existingCampaigns + count));
        }

        [Then(@"(.*) additional Campaigns are returned")]
        public async Task ThenAdditionalCampaignsAreReturned(int count)
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);

            var existingCampaigns = ScenarioContext.Get<int>();
            Assert.That(campaigns.Count, Is.EqualTo(existingCampaigns + count));
        }

        [Then(@"(.*) additional active Campaigns are returned")]
        public void ThenAdditionalActiveCampaignsAreReturned(int count)
        {
            var existingCampaigns = ScenarioContext.Get<int>("existingActiveCampaigns");
            var totalActiveCampaigns = ScenarioContext.Get<int>("totalActiveCampaigns");
            Assert.That(totalActiveCampaigns, Is.EqualTo(existingCampaigns + count));
        }

        [Then(@"no Campaigns are returned")]
        public async void ThenNoCampaignsAreReturned()
        {
            var campaigns = await Api.GetAll().ConfigureAwait(false);
            Assert.Zero(campaigns.Count);
        }

        [When(@"I update Campaign by external id")]
        public async Task WhenIUpdateCampaignByExternalId()
        {
            var campaigns = ScenarioContext.Get<List<CreateCampaign>>();
            var campaign = campaigns.FirstOrDefault();
            campaign.CampaignGroup = "CampaignGroupUpdate1";
            var updatedCampaign = await Api.Put(campaign.ExternalId, campaign).ConfigureAwait(false);
            ScenarioContext.Set(updatedCampaign);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"updated Campaign is returned")]
        public async Task ThenUpdatedCampaignIsReturned()
        {
            var updatedCampaign = ScenarioContext.Get<Campaign>();
            var campaign = await Api.GetByExternalRef(updatedCampaign.ExternalId).ConfigureAwait(false);
            Assert.AreEqual(campaign.ExternalId, updatedCampaign.ExternalId);
        }

        [Given(@"I have an invalid DesiredPercentageSplit in Length of Campaign")]
        public void GivenIHaveAnInvalidDesiredPercentageSplitInLengthCampaign()
        {
            var campaign = Fixture.Create<CreateCampaign>();
            campaign = ChangeDesiredPercentageSplitInFirstDayPartLength(campaign, 10);

            ScenarioContext.Set(campaign, "CampaignWithInvalidPercentageSplitValueInFirstDayPartLength");
        }

        [Given(@"I have an invalid CurrentPercentageSplit in Length of Campaign")]
        public void GivenIHaveAnInvalidCurrentPercentageSplitInLengthCampaign()
        {
            var campaign = Fixture.Create<CreateCampaign>();
            campaign = ChangeCurrentPercentageSplitInFirstDayPartLength(campaign, 10);

            ScenarioContext.Set(campaign, "CampaignWithInvalidPercentageSplitValueInFirstDayPartLength");
        }

        [When(@"I update Campaign with invalid percentage split value in day part length")]
        public async Task WhenIUpdateCampaign()
        {
            var payload =
                ScenarioContext.Get<CreateCampaign>("CampaignWithInvalidPercentageSplitValueInFirstDayPartLength");

            var response = await CreateCampaigns(new List<CreateCampaign>() {payload}).ConfigureAwait(false);

            ScenarioContext.Set(response, "CampaignWithInvalidPercentageSplitValueInFirstDayPartLengthUpdateResult");
        }

        [Then(@"error response is returned")]
        public void ThenErrorResponseIsReturned()
        {
            var response =
                ScenarioContext.Get<ApiResponse<object>>(
                    "CampaignWithInvalidPercentageSplitValueInFirstDayPartLengthUpdateResult");

            Assert.AreNotEqual(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<ApiResponse<object>> CreateCampaigns(List<CreateCampaign> campaigns)
        {
            var demographic = ScenarioContext.Get<Demographic>();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var breakType = ScenarioContext.Get<List<string>>("BreakTypes");
            foreach (var campaign in campaigns)
            {
                var product = Fixture.Create<Product>();
                await _productsApi.Create(new List<Product> { product }).ConfigureAwait(false);
                await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);

                campaign.Product = product.Externalidentifier;
                campaign.DemoGraphic = demographic.ExternalRef;
                campaign.BreakType = breakType;

                foreach (var campaignTarget in campaign.SalesAreaCampaignTarget)
                {
                    campaignTarget.SalesArea = salesArea.Name;
                    campaignTarget.SalesAreaGroup.SalesAreas = new List<string> { salesArea.Name };
                }

                foreach (var programme in campaign.ProgrammeRestrictions)
                {
                    programme.SalesAreas = new List<string> { salesArea.Name };
                }

                foreach (var tr in campaign.TimeRestrictions)
                {
                    tr.SalesAreas = new List<string> { salesArea.Name };
                }
            }

            var response = await Api.Create(campaigns).ConfigureAwait(false);
            ScenarioContext.Set(campaigns);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);

            return response;
        }

        private static CreateCampaign ChangeDesiredPercentageSplitInFirstDayPartLength(CreateCampaign payload,
            decimal desiredPercentageSplitValue)
        {
            var dayPartLength = GetFirstDayPartLength(payload);

            if (dayPartLength is null)
            {
                throw new ArgumentException($"Provided campaign {nameof(payload)} does not have day part length");
            }

            dayPartLength.DesiredPercentageSplit = desiredPercentageSplitValue;

            return payload;
        }

        private static CreateCampaign ChangeCurrentPercentageSplitInFirstDayPartLength(CreateCampaign payload,
            decimal currentPercentageSplitValue)
        {
            var dayPartLength = GetFirstDayPartLength(payload);

            if (dayPartLength is null)
            {
                throw new ArgumentException($"Provided campaign {nameof(payload)} does not have day part length");
            }

            dayPartLength.CurrentPercentageSplit = currentPercentageSplitValue;

            return payload;
        }

        private static DayPartLength GetFirstDayPartLength(CreateCampaign payload)
        {
            return payload.SalesAreaCampaignTarget?.FirstOrDefault()?.CampaignTargets?.FirstOrDefault()
                ?.StrikeWeights?.FirstOrDefault()?.DayParts?.FirstOrDefault()?.Lengths.FirstOrDefault();
        }
    }
}
