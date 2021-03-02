using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Metadata;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Metadata
{
    [Binding]
    public class MetadataSteps : BaseSteps<IMetadataApi>
    {
        public MetadataSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I have a valid Break Types ""(.*)""")]
        public async Task GivenIHaveAValidBreakTypes(List<string> breakTypes)
        {
            await Api.Create(MetadataType.BreakTypes, breakTypes)
                .ConfigureAwait(false);
            ScenarioContext.Set(breakTypes, "BreakTypes");
        }

        [Given(@"I have added (.*) values for type (.*)")]
        public async Task IHaveAddedMetadataValuesForType(int count, MetadataType key)
        {
            await CreateMetadata(key, count).ConfigureAwait(false);
            ScenarioContext.Set(key);
        }

        [When(@"I add (.*) Metadata values")]
        public async Task WhenIMetadataValues(int count)
        {
            var key = ScenarioContext.Get<MetadataType>();
            await CreateMetadata(key, count).ConfigureAwait(false);
        }

        [When(@"I delete Metadata for type (.*)")]
        public async Task WhenIDeleteMetadataForType(MetadataType key)
        {
            await Api.Delete(key).ConfigureAwait(false);
        }

        [Then(@"only (.*) Metadata values are returned")]
        public async Task ThenAdditionalMetadataValuesAreReturned(int count)
        {
            var key = ScenarioContext.Get<MetadataType>();
            var values = await Api.GetByKey(key).ConfigureAwait(false);
            Assert.AreEqual(values.Count(), count);
        }

        [Then(@"no Metadata values are returned")]
        public async Task ThenNoMetadataValuesAreReturned()
        {
            var key = ScenarioContext.Get<MetadataType>();
            var values = await Api.GetByKey(key).ConfigureAwait(false);
            Assert.IsNull(values);
        }

        private async Task CreateMetadata(MetadataType key, int count)
        {
            var values = Fixture.CreateMany<string>(count).ToList();
            await Api.Create(key, values).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }
    }
}
