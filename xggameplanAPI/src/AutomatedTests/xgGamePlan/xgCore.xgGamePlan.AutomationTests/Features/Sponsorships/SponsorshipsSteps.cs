using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Clashes;
using xgCore.xgGamePlan.ApiEndPoints.Models.Products;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Coordinators;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Sponsorships
{
    [Binding]
    public class SponsorshipsSteps : BaseSteps<ISponsorshipsApi>
    {
        private readonly SponsorshipsCoordinator _sponsorshipsCoordinator;

        public SponsorshipsSteps(
            ScenarioContext scenarioContext,
            SponsorshipsCoordinator sponsorshipsCoordinator
            )
            : base(scenarioContext)
        {
            _sponsorshipsCoordinator = sponsorshipsCoordinator;
        }

        [Given(@"I know how many Sponsorships there are")]
        public async Task GivenIKnowHowManySponsorshipsThereAre()
        {
            var sponsorships = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(sponsorships.Count, "sponsorshipsCount");
        }

        [Given(@"I know there are some Sponsorships")]
        public async Task GivenIKnowThereAreSomeSponsorships()
        {
            var sponsorships = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(sponsorships.Count, "sponsorshipsCount");
            Assert.That(sponsorships.Count, Is.GreaterThan(0));
        }

        [When(@"I add (.*) Sponsorships")]
        public async Task<IEnumerable<CreateSponsorshipModel>> WhenIAddSponsorships(int count)
        {
            var createSponsorshipModels = CreateSponsorshipModels(count);

            await Api.Create(createSponsorshipModels).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);

            return createSponsorshipModels;
        }
        
        [Given(@"I add an Sponsorship")]
        public async Task WhenIAddSponsorship()
        {
            var createSponsorshipModel = CreateSponsorshipModel();

            await Api.Create(new[] {createSponsorshipModel}).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);

            ScenarioContext.Set(createSponsorshipModel.ExternalReferenceId, "sponsorshipsExternalReference");
        }

        [When(@"I delete that Sponsorship")]
        public async Task WhenIDeleteThatSponsorship()
        {
            var externalReferenceId = ScenarioContext.Get<string>("sponsorshipsExternalReference");
            await Api.Delete(externalReferenceId).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I delete all Sponsorships")]
        public async Task WhenIDeleteAllSponsorships()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Sponsorships are returned")]
        public async Task ThenAdditionalSponsorshipsAreReturned(int count)
        {
            var sponsorships = await Api.GetAll().ConfigureAwait(false);
            int existingSponsorships = ScenarioContext.Get<int>("sponsorshipsCount");
            Assert.That(sponsorships.Count, Is.EqualTo(existingSponsorships + count));
        }

        [Then(@"that Sponsorship is not returned")]
        public async Task ThenThatSponsorshipIsNotReturned()
        {
            var externalReferenceId = ScenarioContext.Get<string>("sponsorshipsExternalReference");
            var sponsorship = await GetSponsorship(externalReferenceId).ConfigureAwait(false);

            Assert.That(sponsorship, Is.Null);
        }

        [Then(@"No Sponsorships are returned")]
        public async Task ThenNoSponsorshipsAreReturned()
        {
            var sponsorships = await Api.GetAll().ConfigureAwait(false);
            Assert.That(sponsorships.Count, Is.EqualTo(0));
        }
        
        [Given(@"I add an Sponsorship with (.*) SponsoredItem")]
        public async Task GivenIAddAnSponsorshipWithSponsoredItem(int p0)
        {
            var sponsorship = CreateSponsorshipModel();
            var sponsoredItems = new List<CreateSponsoredItemModel>();
            for (int i = 0; i < p0; i++)
            {
                var sponsoredItem = CreateSponsoredItemModel();
                sponsoredItems.Add(sponsoredItem);
            }
            sponsorship.SponsoredItems = sponsoredItems;

            await Api.Create(new[] {sponsorship}).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);

            ScenarioContext.Set(sponsorship);
        }
        
        [When(@"I try to update Sponsorship to add another SponsoredItem")]
        public async Task WhenITryToUpdateSponsorshipToAddAnotherSponsoredItem()
        {
            var sponsorship = ScenarioContext.Get<CreateSponsorshipModel>();

            var sponsoredItem = CreateSponsoredItemModel();
            var sponsoredItemModels = sponsorship.SponsoredItems.ToList();
            sponsoredItemModels.Add(sponsoredItem);
            sponsorship.SponsoredItems = sponsoredItemModels;

            await Api.Update(sponsorship).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);

            ScenarioContext.Set(sponsorship.ExternalReferenceId, "sponsorshipsExternalReference");
        }

        [Then(@"I get Sponsorship")]
        public async Task ThenIGetSponsorship()
        {
            var externalReferenceId = ScenarioContext.Get<string>("sponsorshipsExternalReference");
            var sponsorship = await GetSponsorship(externalReferenceId).ConfigureAwait(false);

            ScenarioContext.Set(sponsorship);
        }

        [Then(@"Sponsorship has (.*) SponsoredItems")]
        public void ThenSponsorshipHasSponsoredItems(int p0)
        {
            var sponsorship = ScenarioContext.Get<CreateSponsorshipModel>();
            Assert.AreEqual(2, sponsorship.SponsoredItems.Count());
        }

        private async Task<SponsorshipModel> GetSponsorship(string aExternalReferenceId)
        {
            var sponsorships = await Api.GetAll().ConfigureAwait(false);
            var sponsorship = sponsorships.FirstOrDefault(s => s.ExternalReferenceId == aExternalReferenceId.ToString());
            return sponsorship;
        }

        private IEnumerable<CreateSponsorshipModel> CreateSponsorshipModels(int count)
        {
            var products = ScenarioContext.Get<IEnumerable<Product>>();
            var productExtIdentifiers = products.Select(x => x.Externalidentifier).ToArray();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var сlash = ScenarioContext.Get<Clash>();
            var advertiserIdentifier = products.First().AdvertiserIdentifier;

            var createSponsorshipModels = _sponsorshipsCoordinator.CreateSponsorshipModel(count, productExtIdentifiers, salesArea.Name, advertiserIdentifier, сlash.Externalref)
                .ToArray();
            return createSponsorshipModels;
        }

        private CreateSponsorshipModel CreateSponsorshipModel()
        {
            var sponsorships = CreateSponsorshipModels(1);
            return sponsorships.First();
        }

        private CreateSponsoredItemModel CreateSponsoredItemModel()
        {
            var products = ScenarioContext.Get<IEnumerable<Product>>();
            var productExtIdentifiers = products.Select(x => x.Externalidentifier).ToArray();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var сlash = ScenarioContext.Get<Clash>();
            var advertiserIdentifier = products.First().AdvertiserIdentifier;

            var createSponsorshipModels = _sponsorshipsCoordinator.CreateSponsoredItemModels(productExtIdentifiers, salesArea.Name, advertiserIdentifier, сlash.Externalref);
            return createSponsorshipModels;
        }
    }
}
