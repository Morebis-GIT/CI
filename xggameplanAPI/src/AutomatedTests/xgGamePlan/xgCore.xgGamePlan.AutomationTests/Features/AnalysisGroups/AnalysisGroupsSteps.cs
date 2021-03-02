using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.AnalysisGroups;
using xgCore.xgGamePlan.ApiEndPoints.Models.Clashes;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.AnalysisGroups
{
    [Binding]
    public class AnalysisGroupsSteps : BaseSteps<IAnalysisGroupApi>
    {
        public AnalysisGroupsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many analysis groups there are")]
        public async Task GivenIKnowHowManyAnalysisGroupThereAre()
        {
            var entities = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(entities.Count());
        }

        [Given(@"I have a valid analysis group")]
        public async Task GivenIHaveAValidAnalysisGroup()
        {
            var model = BuildValidAnalysisGroup(1).First();
            var result = await Api.Create(model).ConfigureAwait(false);

            ScenarioContext.Set(result);
        }

        [When(@"I add (.*) analysis group")]
        public async Task WhenIAddAnalysisGroup(int count)
        {
            var entities = BuildValidAnalysisGroup(count);
            await Task.WhenAll(entities.Select(x => Api.Create(x))).ConfigureAwait(false);
        }

        [When(@"I update analysis group by Id")]
        public async Task WhenIUpdateAnalysisGroupById()
        {
            var inMemoryModel = ScenarioContext.Get<AnalysisGroupModel>();
            var newModel = BuildValidAnalysisGroup(1).First();

            newModel.Id = inMemoryModel.Id;
            newModel.Name = Guid.NewGuid().ToString();
            ScenarioContext.Set(newModel.Name);

            await Api.Update(newModel.Id, newModel).ConfigureAwait(false);
        }

        [When(@"I delete analysis group")]
        public async Task WhenIDeleteAnalysisGroup()
        {
            var id = ScenarioContext.Get<AnalysisGroupModel>().Id;
            await Api.Delete(id).ConfigureAwait(false);
        }

        [When(@"I get analysis group by Id")]
        public async Task WhenIGetAnalysisGroup()
        {
            var id = ScenarioContext.Get<AnalysisGroupModel>().Id;
            var result = await Api.GetById(id).ConfigureAwait(false);
            ScenarioContext.Set(result);
        }

        [Then(@"analysis group is updated")]
        public async Task ThenAnalysisGroupIsUpdated()
        {
            var id = ScenarioContext.Get<AnalysisGroupModel>().Id;
            var entity = await Api.GetById(id).ConfigureAwait(false);
            string name = ScenarioContext.Get<string>();

            Assert.AreEqual(entity.Name, name);
        }

        [Then(@"analysis group is deleted")]
        public async Task ThenAnalysisGroupIsDeleted()
        {
            var id = ScenarioContext.Get<AnalysisGroupModel>().Id;

            try
            {
                await Api.GetById(id).ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                Assert.That(e.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [Then(@"analysis group is returned")]
        public void ThenAnalysisGroupIsReturned()
        {
            var result = ScenarioContext.Get<AnalysisGroupModel>();
            Assert.IsNotNull(result);
        }

        [Then(@"(.*) additional analysis groups are returned")]
        public async Task ThenAdditionalAnalysisGroupsAreReturned(int count)
        {
            var analysisGroups = await Api.GetAll().ConfigureAwait(false);

            int existingAnalysisGroupCount = ScenarioContext.Get<int>();
            Assert.That(analysisGroups.Count, Is.EqualTo(existingAnalysisGroupCount + count));
        }

        private IEnumerable<CreateAnalysisGroupModel> BuildValidAnalysisGroup(int count)
        {
            var clash = ScenarioContext.Get<Clash>();
            var filter = Fixture.Build<CreateAnalysisGroupFilterModel>()
                .OmitAutoProperties()
                .With(x => x.ClashExternalRefs, new HashSet<string> {clash.Externalref})
                .Create();

            return Fixture.Build<CreateAnalysisGroupModel>()
                .With(p => p.Id, 1)
                .With(p => p.Filter, filter)
                .CreateMany(count);
        }
    }
}
