using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.DeliveryCappingGroup;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.DeliveryCappingGroup
{
    [Binding]
    public class DeliveryCappingGroupSteps : BaseSteps<IDeliveryCappingGroupApi>
    {
        public DeliveryCappingGroupSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many delivery capping groups there are")]
        public async Task GivenIKnowHowManyDeliveryCappingGroupThereAre()
        {
            var entities = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(entities.Count());
        }

        [Given(@"I have a valid delivery capping group")]
        public async Task GivenIHaveAValidDeliveryCappingGroup()
        {
            var entity = BuildValidDeliveryCappingGroup(1).First();
            entity = await Api.Create(entity).ConfigureAwait(false);
            ScenarioContext.Set(entity);
        }

        [When(@"I add (.*) delivery capping group")]
        public async Task WhenIAddDeliveryCappingGroup(int count)
        {
            var entities = BuildValidDeliveryCappingGroup(count);
            await Task.WhenAll(entities.Select(x => Api.Create(x))).ConfigureAwait(false);
        }

        [When(@"I update delivery capping group by Id")]
        public async Task WhenIUpdateDeliveryCappingGroupById()
        {
            var entity = ScenarioContext.Get<DeliveryCappingGroupModel>();

            int newPercentageValue = new Random().Next(0, 999);
            entity.Percentage = newPercentageValue;
            ScenarioContext.Set(newPercentageValue);

            await Api.Update(entity.Id, entity).ConfigureAwait(false);
        }

        [When(@"I delete delivery capping group")]
        public async Task WhenIDeleteDeliveryCappingGroup()
        {
            var id = ScenarioContext.Get<DeliveryCappingGroupModel>().Id;
            await Api.Delete(id).ConfigureAwait(false);
        }

        [When(@"I get delivery capping group by Id")]
        public async Task WhenIGetDeliveryCappingGroup()
        {
            var id = ScenarioContext.Get<DeliveryCappingGroupModel>().Id;
            var result = await Api.GetById(id).ConfigureAwait(false);
            ScenarioContext.Set(result);
        }

        [Then(@"delivery capping group is updated")]
        public async Task ThenDeliveryCappingGroupIsUpdated()
        {
            var id = ScenarioContext.Get<DeliveryCappingGroupModel>().Id;
            var entity = await Api.GetById(id).ConfigureAwait(false);
            int percentagesValue = ScenarioContext.Get<int>();

            Assert.AreEqual(entity.Percentage, percentagesValue);
        }

        [Then(@"delivery capping group is deleted")]
        public async Task ThenDeliveryCappingGroupIsDeleted()
        {
            var id = ScenarioContext.Get<DeliveryCappingGroupModel>().Id;

            try
            {
                await Api.GetById(id).ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                Assert.That(e.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [Then(@"delivery capping group is returned")]
        public void ThenDeliveryCappingGroupIsReturned()
        {
            var result = ScenarioContext.Get<DeliveryCappingGroupModel>();
            Assert.IsNotNull(result);
        }

        [Then(@"(.*) additional delivery capping groups are returned")]
        public async Task ThenAdditionalDeliveryCappingGroupsAreReturned(int count)
        {
            var deliveryCappingGroups = await Api.GetAll().ConfigureAwait(false);

            int existingDeliveryCappingGroupCount = ScenarioContext.Get<int>();
            Assert.That(deliveryCappingGroups.Count, Is.EqualTo(existingDeliveryCappingGroupCount + count));
        }

        private IEnumerable<DeliveryCappingGroupModel> BuildValidDeliveryCappingGroup(int count)
        {
            var random = new Random();

            return Fixture
                .Build<DeliveryCappingGroupModel>()
                .With(p => p.Id, 0)
                .With(p => p.Description, "test")
                .With(p => p.Percentage, random.Next(1, 999))
                .With(p => p.ApplyToPrice, true)
                .CreateMany(count);
        }
    }
}
