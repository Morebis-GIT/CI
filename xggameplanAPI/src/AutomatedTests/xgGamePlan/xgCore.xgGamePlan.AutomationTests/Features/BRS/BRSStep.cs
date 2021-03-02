using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.BRS;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Infrastructure;

namespace xgCore.xgGamePlan.AutomationTests.Features.BRS
{
    [Binding]
    public class BRSStep : BaseSteps<IBRSTemplateApi>
    {
        private readonly IMapper _mapper;

        public BRSStep(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            _mapper = AutoMapperHelper.GetMapper();
        }

        [Given(@"I know how many BRS configuration templates there are")]
        public async Task GivenIKnowHowManyBRSConfigurationTemplateThereAre()
        {
            var entities = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(entities.Count());
        }

        [Given(@"I have a valid BRS configuration template")]
        public async Task GivenIHaveAValidBRSConfigurationTemplate()
        {
            var entity = BuildValidInputBRSConfigurationTemplateModel(1).First();
            var result = await Api.Create(entity).ConfigureAwait(false);
            ScenarioContext.Set(result);
        }

        [When(@"I add (.*) BRS configuration template")]
        public async Task WhenIAddBRSConfigurationTemplate(int count)
        {
            var entities = BuildValidInputBRSConfigurationTemplateModel(count);
            await Task.WhenAll(entities.Select(x => Api.Create(x))).ConfigureAwait(false);
        }

        [When(@"I update BRS configuration template by Id")]
        public async Task WhenIUpdateBRSConfigurationTemplateById()
        {
            var entity = ScenarioContext.Get<BRSConfigurationTemplateModel>();
            entity.Name = $"[NEW NAME: {Guid.NewGuid()}]";
            ScenarioContext.Set(entity.Name);

            var input = _mapper.Map<CreateOrUpdateBRSConfigurationTemplateModel>(entity);
            ScenarioContext.Set(input);

            await Api.Update(input.Id, input).ConfigureAwait(false);
        }

        [When(@"I delete BRS configuration template")]
        public async Task WhenIDeleteBRSConfigurationTemplate()
        {
            var id = ScenarioContext.Get<BRSConfigurationTemplateModel>().Id;
            await Api.Delete(id).ConfigureAwait(false);
        }

        [When(@"I get BRS configuration template by Id")]
        public async Task WhenIGetBRSConfigurationTemplate()
        {
            var id = ScenarioContext.Get<BRSConfigurationTemplateModel>().Id;
            var result = await Api.GetById(id).ConfigureAwait(false);
            ScenarioContext.Set(result);
        }

        [Then(@"BRS configuration template is updated")]
        public async Task ThenBRSConfigurationTemplateIsUpdated()
        {
            var id = ScenarioContext.Get<CreateOrUpdateBRSConfigurationTemplateModel>().Id;
            var entity = await Api.GetById(id).ConfigureAwait(false);
            var newName = ScenarioContext.Get<string>();

            Assert.AreEqual(entity.Name, newName);
        }

        [Then(@"BRS configuration template is deleted")]
        public async Task ThenBRSConfigurationTemplateIsDeleted()
        {
            var id = ScenarioContext.Get<BRSConfigurationTemplateModel>().Id;

            try
            {
                await Api.GetById(id).ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                Assert.That(e.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [Then(@"BRS configuration template is returned")]
        public void ThenBRSConfigurationTemplateIsReturned()
        {
            var result = ScenarioContext.Get<BRSConfigurationTemplateModel>();
            Assert.IsNotNull(result);
        }
        
        [Then(@"(.*) additional BRS configuration template are returned")]
        public async Task ThenAdditionalBRSConfigurationTemplatesAreReturned(int count)
        {
            var entities = await Api.GetAll().ConfigureAwait(false);
            int existingEntitiesCount = ScenarioContext.Get<int>();
            Assert.That(entities.Count, Is.EqualTo(existingEntitiesCount + count));
        }

        [When(@"I set as default any BRS configuration template by id")]
        public async Task WhenISetAsDefaultBRSConfigurationTemplateById()
        {
            var entities = await Api.GetAll().ConfigureAwait(false);
            var id = entities.First().Id;
            await Api.SetAsDefault(id).ConfigureAwait(false);
            ScenarioContext.Set(id);
        }

        [Then(@"BRS configuration template is default")]
        public async Task ThenBRSConfigurationTemplateIsDefault()
        {
            var id = ScenarioContext.Get<int>();
            var entity = await Api.GetById(id).ConfigureAwait(false);
            Assert.True(entity.IsDefault);
        }

        private IEnumerable<CreateOrUpdateBRSConfigurationTemplateModel> BuildValidInputBRSConfigurationTemplateModel(int count)
        {
            return Fixture
                .Build<CreateOrUpdateBRSConfigurationTemplateModel>()
                .With(p => p.Id, 0)
                .With(p => p.Name, $"test-{Guid.NewGuid()}")
                .With(p => p.KPIConfigurations, KPIs.Select(kpi => new BRSConfigurationForKPIModel
                {
                    KPIName = kpi,
                    PriorityId = 4
                }).ToList())
                .CreateMany(count);
        }

        private List<string> KPIs = new List<string>
        {
            "percent95to105",
            "percentbelow75",
            "averageEfficiency",
            "totalSpotsBooked",
            "remainaudience",
            "remainingAvailability",
            "standardAverageCompletion",
            "weightedAverageCompletion"
        };
    }
}
