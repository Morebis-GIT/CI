using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Channels;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Channels
{
    [Binding]
    public class ChannelsSteps : BaseSteps<IChannelsApi>
    {
        public ChannelsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many channels there are")]
        public async Task GivenIKnowHowManyChannelsThereAre()
        {
            var channels = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(channels.Count());
        }

        [When(@"I add (.*) channels")]
        public async Task WhenIAddChannels(int count)
        {
            var channels = Fixture.Build<Channel>().CreateMany(count).ToList();
            var createTasks = new List<Task<ApiErrorResult>>();
            foreach(var c in channels)
            {
                var t = Api.Create(c);
                createTasks.Add(t);
            }
            await Task.WhenAll(createTasks).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I delete (.*) channels")]
        public async Task WhenIDeleteChannels(int count)
        {
            var existingChannels = await Api.GetAll().ConfigureAwait(false);
            var channelsToDelete = existingChannels.Take(count);
            var deleteTasks = new List<Task<ApiErrorResult>>();
            foreach (var channel in channelsToDelete)
            {
                await Api.Delete(channel.Uid).ConfigureAwait(false);
            }
            await Task.WhenAll(deleteTasks).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"(.*) additional channels are returned")]
        public async Task ThenAdditionalChannelsAreReturned(int count)
        {
            var existingChannels = await Api.GetAll().ConfigureAwait(false);
            var knownCount = ScenarioContext.Get<int>();

            Assert.That(existingChannels.Count, Is.EqualTo(knownCount + count));
        }
    }
}
