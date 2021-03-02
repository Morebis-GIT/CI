using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Coordinators;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Passes
{
    [Binding]
    public class PassesSteps : BaseSteps<IPassesApi>
    {
        private readonly PassesCoordinator _passesCoordinator;

        public PassesSteps(ScenarioContext context, PassesCoordinator passesCoordinator) : base(context)
        {
            _passesCoordinator = passesCoordinator;
        }

        [Given(@"I know how many Passes there are")]
        public async Task GivenIKnowHowManyPassesThereAre()
        {
            var passes = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(passes == null ? 0 : passes.Count(), "passesCount");
        }

        [When(@"I add (.*) Passes")]
        public async Task WhenIAddPasses(int count)
        {
            await _passesCoordinator.CreatePassesAsync(count).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Passes are returned")]
        public async Task ThenAdditionalPassesAreReturned(int count)
        {
            var existingPasses = await Api.GetAll().ConfigureAwait(false);
            int previousPassesCount = ScenarioContext.Get<int>("passesCount");

            Assert.That(existingPasses.Count() == previousPassesCount + count);
        }

        [Given(@"I have added a Pass")]
        public async Task GivenIHaveAddedPass()
        {
            ScenarioContext.Set((await _passesCoordinator.CreatePassesAsync(1).ConfigureAwait(false)).Single(),
                "createdPass");
        }

        [When(@"I request my Pass by ID")]
        public async Task WhenIRequestMyPassByIDAsync()
        {
            int passId = ScenarioContext.Get<Pass>("createdPass").Id;
            var pass = await Api.GetById(passId).ConfigureAwait(false);
            ScenarioContext.Set(pass.Id, "returnedPassId");
        }

        [Then(@"requested Pass with ID is returned")]
        public void ThenRequestedPassWithIDIsReturned()
        {
            int createdPassId = ScenarioContext.Get<Pass>("createdPass").Id;
            int returnedPassId = ScenarioContext.Get<int>("returnedPassId");

            Assert.That(createdPassId == returnedPassId);
        }

        [When(@"I update Pass by ID")]
        public async Task WhenIUpdatePassByID()
        {
            var createdPass = ScenarioContext.Get<Pass>("createdPass");
            var pass = _passesCoordinator.BuildPasses(1, true).Single();
            pass.Id = createdPass.Id;

            var updatedPass = await Api.Update(createdPass.Id, pass).ConfigureAwait(false);
            ScenarioContext.Set(updatedPass, "updatedPass");
        }

        [Then(@"updated Pass is returned")]
        public void ThenUpdatedPassIsReturned()
        {
            var createdPass = ScenarioContext.Get<Pass>("createdPass");
            var updatedPass = ScenarioContext.Get<Pass>("updatedPass");

            Assert.That(createdPass.Id == updatedPass.Id && createdPass.DateModified != updatedPass.DateModified);
        }

        [When(@"I remove my Pass by ID")]
        public async Task WhenIRemoveMyPassByID()
        {
            int createdPassId = ScenarioContext.Get<Pass>("createdPass").Id;
            await Api.DeleteById(createdPassId).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Pass with ID is returned")]
        public async Task ThenNoPassWithIDIsReturned()
        {
            int createdPassId = ScenarioContext.Get<Pass>("createdPass").Id;
            try
            {
                await Api.GetById(createdPassId).ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                Assert.That(e.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [When(@"I search my Pass by Name")]
        public async Task WhenISearchMyPassByName()
        {
            string createdPassName = ScenarioContext.Get<Pass>("createdPass").Name;
            var pass = await Api.Search(new PassSearchQueryModel
            {
                Name = createdPassName
            }).ConfigureAwait(false);

            ScenarioContext.Set(pass.Items.First(), "foundPass");
        }

        [Then(@"requested Pass with Name is found")]
        public void ThenRequestedPassWithNameIsFound()
        {
            var createdPass = ScenarioContext.Get<Pass>("createdPass");
            var foundPass = ScenarioContext.Get<Pass>("foundPass");

            Assert.That(createdPass.Id == foundPass.Id && createdPass.Name == foundPass.Name);
        }
    }
}
