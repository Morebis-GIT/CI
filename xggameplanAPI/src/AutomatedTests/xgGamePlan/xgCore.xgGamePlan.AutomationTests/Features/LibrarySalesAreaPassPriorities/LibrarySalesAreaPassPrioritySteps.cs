using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.LibrarySalesAreaPassPriorities;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.LibrarySalesAreaPassPriorities
{
    [Binding]
    public class LibrarySalesAreaPassPrioritySteps : BaseSteps<ILibrarySalesAreaPassPrioritiesApi>
    {
        public LibrarySalesAreaPassPrioritySteps(ScenarioContext context) : base(context)
        {
        }

        [Given(@"I know how many SalesAreaPassPriorities there are")]
        public async Task GivenIKnowHowManySalesAreaPassPrioritiesThereAre()
        {
            int count = (await Api.GetAll().ConfigureAwait(false)).Count();
            ScenarioContext.Set(count, "count");
        }

        [When(@"I add (.*) SalesAreaPassPriorities")]
        public async Task WhenIAddSalesAreaPassPriorities(int count)
        {
            await CreateSalesAreaPassPriorities(count).ConfigureAwait(false);
        }

        [Then(@"(.*) additional SalesAreaPassPriorities are returned")]
        public async Task ThenAdditionalSalesAreaPassPrioritiesAreReturned(int count)
        {
            int previousCount = ScenarioContext.Get<int>("count");
            int currentCount = (await Api.GetAll().ConfigureAwait(false)).Count();

            Assert.AreEqual(previousCount + count, currentCount);
        }

        [Given(@"I have created new SalesAreaPassPriority")]
        public async Task GivenIHaveCreatedNewSalesAreaPassPriority()
        {
            await CreateSalesAreaPassPriorities(1).ConfigureAwait(false);
        }

        [When(@"I request created SalesAreaPassPriority by id")]
        public async Task WhenIRequestCreatedSalesAreaPassPriorityById()
        {
            var requestedSalesAreaPassPriority = await RequestByCreatedId().ConfigureAwait(false);
            ScenarioContext.Set(requestedSalesAreaPassPriority, "requested");
        }

        [Then(@"created SalesAreaPassPriority is returned")]
        public void ThenCreatedSalesAreaPassPriorityIsReturned()
        {
            var createdModel = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("created");
            var requestedModel = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("requested");
            
            Assert.AreEqual(createdModel, requestedModel);
        }

        [When(@"I update SalesAreaPassPriority")]
        public async Task WhenIUpdateSalesAreaPassPriority()
        {
            var createdModel = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("created");
            var salesArea = ScenarioContext.Get<SalesArea>();
            var model = new UpdateLibrarySalesAreaPassPriorityModel()
            {
                Uid = createdModel.Uid,
                DaysOfWeek = "0000001",
                StartTime = "10:01:01",
                EndTime = "18:00:59",
                Name = $"Updated{createdModel.Name}",
                SalesAreaPriorities = new List<SalesAreaPriorityModel>()
                {
                    new SalesAreaPriorityModel()
                    {
                        SalesArea = salesArea.Name,
                        Priority = SalesAreaPriorityType.Priority2
                    }
                }
            };

            var updatedModel = await Api.Update(model.Uid, model).ConfigureAwait(false);
            ScenarioContext.Set(updatedModel, "updated");
        }

        [Then(@"updated SalesAreaPassPriority is returned")]
        public void ThenUpdatedSalesAreaPassPriorityIsReturned()
        {
            var createdModel = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("created");
            var updatedModel = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("updated");
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(createdModel.Uid, updatedModel.Uid);
                Assert.AreNotEqual(createdModel, updatedModel);
            });
        }

        [When(@"I delete created SalesAreaPassPriority by id")]
        public async Task WhenIDeleteCreatedSalesAreaPassPriorityById()
        {
            var id = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("created").Uid;
            await Api.Delete(id).ConfigureAwait(false);
        }

        [Then(@"no SalesAreaPassPriority is returned")]
        public void ThenNoSalesAreaPassPriorityIsReturned()
        {
            bool deleted = ScenarioContext.Get<bool>("deleted");
            Assert.AreEqual(deleted, true);
        }

        [Given(@"I know initial default SalesAreaPassPriority")]
        public async Task GivenIKnowInitialDefaultSalesAreaPassPriority()
        {
            try
            {
                var defaultModel = await Api.GetDefault().ConfigureAwait(false);
                ScenarioContext.Set(defaultModel, "default");
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound){}
        }

        [When(@"I set as default created SalesAreaPassPriority")]
        public async Task WhenISetAsDefaultCreatedSalesAreaPassPriority()
        {
            var id = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("created").Uid;
            await Api.SetDefault(id).ConfigureAwait(false);
        }

        [When(@"I get default SalesAreaPassPriority")]
        public async Task WhenIGetDefaultSalesAreaPassPriority()
        {
            var newDefault = await Api.GetDefault().ConfigureAwait(false);
            ScenarioContext.Set(newDefault, "newDefault");
        }

        [Then(@"updated default SalesAreaPassPriority is returned")]
        public void ThenUpdatedDefaultSalesAreaPassPriorityIsReturned()
        {

            var oldDefault = ScenarioContext.ContainsKey("default") ? ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("default") : null;
            var newDefault = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("newDefault");

            Assert.AreNotEqual(oldDefault, newDefault);
        }
        
        private async Task<LibrarySalesAreaPassPriorityModel> RequestByCreatedId()
        {
            ScenarioContext.Set(false, "deleted");
            var id = ScenarioContext.Get<LibrarySalesAreaPassPriorityModel>("created").Uid;
            try
            {
                return await Api.GetById(id).ConfigureAwait(false);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ScenarioContext.Set(true, "deleted");
                return null;
            }
        }

        private async Task CreateSalesAreaPassPriorities(int count)
        {
            var salesArea = ScenarioContext.Get<SalesArea>();
            var salesAreaPriorities = new List<SalesAreaPriorityModel>()
            {
                new SalesAreaPriorityModel()
                {
                    SalesArea = salesArea.Name,
                    Priority = SalesAreaPriorityType.Priority1
                }
            };

            var salesAreaPassPriorities = Fixture.Build<CreateLibrarySalesAreaPassPriorityModel>()
                .With(p => p.DaysOfWeek, "1111111")
                .With(p => p.EndTime, "17:59:59")
                .With(p => p.StartTime, "09:00:00")
                .With(p => p.SalesAreaPriorities, salesAreaPriorities)
                .CreateMany(count);

            if (count == 1)
            {
                var createdSalesAreaPassPriority = await Api.Create(salesAreaPassPriorities.FirstOrDefault()).ConfigureAwait(false);
                ScenarioContext.Set(createdSalesAreaPassPriority, "created");
                return;
            }

            foreach (var salesAreaPassPriority in salesAreaPassPriorities)
            {
                await Api.Create(salesAreaPassPriority).ConfigureAwait(false);
            }
        }
    }
}
