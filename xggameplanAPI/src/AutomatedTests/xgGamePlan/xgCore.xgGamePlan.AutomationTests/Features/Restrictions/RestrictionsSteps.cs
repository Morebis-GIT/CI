using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ClearanceCodes;
using xgCore.xgGamePlan.ApiEndPoints.Models.Restrictions;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Restrictions
{
    [Binding]
    public class RestrictionsSteps : BaseSteps<IRestrictionsApi>
    {
        public RestrictionsSteps(ScenarioContext context) : base(context)
        {
        }

        [Given(@"I know how many Restrictions there are")]
        public async Task GivenIKnowHowManyRestrictionsThereAre()
        {
            var restrictions = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(restrictions?.items.Count() ?? 0);
        }

        [When(@"I add (.*) Restrictions")]
        public async Task WhenIAddRestrictions(int count)
        {
            var restrictions = await CreateRestrictions(count).ConfigureAwait(false);
            ScenarioContext.Set(restrictions);
        }

        [Then(@"(.*) additional Restrictions are returned")]
        public async Task ThenAdditionalRestrictionsAreReturned(int additionalCount)
        {
            int previousCount = ScenarioContext.Get<int>();
            int actualCount = (await Api.GetAll().ConfigureAwait(false)).items.Count();

            Assert.AreEqual(actualCount, previousCount + additionalCount);
        }

        [Given(@"I have added (.*) Restrictions")]
        public async Task GivenIHaveAddedRestrictions(int count)
        {
            var restrictions = await CreateRestrictions(count).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(restrictions);
        }

        [When(@"I delete all Restrictions")]
        public async Task WhenIDeleteAllRestrictions()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Restrictions are returned")]
        public async Task ThenNoRestrictionsAreReturned()
        {
            var restrictions = await Api.GetAll().ConfigureAwait(false);
            Assert.IsNull(restrictions.items);
        }

        [Given(@"I have added (.*) Restriction")]
        public async Task GivenIHaveAddedRestriction(int count)
        {
            var restrictions = await CreateRestrictions(count).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(restrictions);
        }

        [When(@"I request my Restriction by ID")]
        public async Task WhenIRequestMyRestrictionByID()
        {
            var firstRestriction = ScenarioContext.Get<IEnumerable<Restriction>>().FirstOrDefault();
            var restriction = await Api.GetById(firstRestriction.Uid).ConfigureAwait(false);
            ScenarioContext.Set(restriction);
        }

        [Then(@"requested Restriction with ID is returned")]
        public void ThenRequestedRestrictionWithIDIsReturned()
        {
            var firstRestriction = ScenarioContext.Get<IEnumerable<Restriction>>().FirstOrDefault();
            var restriction = ScenarioContext.Get<Restriction>();

            Assert.AreEqual(firstRestriction.Uid, restriction.Uid);
        }

        [When(@"I update Restriction by ExternalIdentifier")]
        public async Task WhenIUpdateRestrictionByExternalIdentifier()
        {
            var restriction = ScenarioContext.Get<IEnumerable<Restriction>>().FirstOrDefault();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var clearanceCode = ScenarioContext.Get<ClearanceCode>();

            var newRestriction = BuildRestrictionFixtures(salesArea, clearanceCode, 1).FirstOrDefault();

            var updatedRestriction = await Api.Put(restriction.ExternalIdentifier, newRestriction).ConfigureAwait(false);
            ScenarioContext.Set(updatedRestriction);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I update Restriction by ID")]
        public async Task WhenIUpdateRestrictionById()
        {
            var restriction = ScenarioContext.Get<IEnumerable<Restriction>>().FirstOrDefault();
            var salesArea = ScenarioContext.Get<SalesArea>();
            var clearanceCode = ScenarioContext.Get<ClearanceCode>();

            var newRestriction = BuildRestrictionFixtures(salesArea, clearanceCode, 1).FirstOrDefault();

            var updatedRestriction = await Api.Put(restriction.Uid, newRestriction).ConfigureAwait(false);
            ScenarioContext.Set(updatedRestriction);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"updated Restriction is returned")]
        public async Task ThenUpdatedRestrictionIsReturned()
        {
            var updatedRestriction = ScenarioContext.Get<Restriction>();
            var restriction = await Api.GetById(updatedRestriction.Uid).ConfigureAwait(false);

            Assert.AreEqual(restriction.Uid, updatedRestriction.Uid);
        }

        [When(@"I remove my Restriction by ID")]
        public async Task WhenIRemoveMyRestrictionByID()
        {
            var restriction = ScenarioContext.Get<IEnumerable<Restriction>>().FirstOrDefault();
            await Api.DeleteById(restriction.Uid).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Restriction with ID is returned")]
        public async Task ThenNoRestrictionWithIDIsReturned()
        {
            var restriction = ScenarioContext.Get<IEnumerable<Restriction>>().FirstOrDefault();
            var response = await Api.GetByIdResponse(restriction.Uid).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Wrong expected status code.");
        }

        private async Task<IEnumerable<Restriction>> CreateRestrictions(int count)
        {
            var salesArea = ScenarioContext.Get<SalesArea>();
            var clearanceCode = ScenarioContext.Get<ClearanceCode>();
            var restrictionFixtures = BuildRestrictionFixtures(salesArea, clearanceCode, count);
            var restrictions = await Task.WhenAll(restrictionFixtures.Select(r => Api.Create(r))).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            return restrictions;
        }

        private IEnumerable<Restriction> BuildRestrictionFixtures(SalesArea salesArea, ClearanceCode clearanceCode, int count)
        {
            var restrictions = Fixture.Build<Restriction>()
                .Without(p => p.SalesAreas)
                .Without(p => p.ClearanceCode)
                .Without(p => p.RestrictionDays)
                .Without(p => p.StartDate)
                .Without(p => p.EndDate)
                .Without(p => p.ClashCode)
                .Without(p => p.ProductCode)
                .Without(p => p.ClockNumber)
                .Without(p => p.ExternalProgRef)
                .Without(p => p.ProgrammeCategory)
                .Without(p => p.ProgrammeClassification)
                .Without(p => p.RestrictionBasis)
                .Without(p => p.RestrictionType)
                .Without(p => p.IndexType)
                .Without(p => p.IndexThreshold)
                .Do(p =>
                {
                    p.SalesAreas = new List<string>();
                    p.SalesAreas.ToList().Add(salesArea.Name);
                    p.ClearanceCode = clearanceCode.Code;
                    p.RestrictionDays = "1111111";
                    p.StartDate = Fixture.Create<DateTime>();
                    p.EndDate = p.StartDate.AddDays(1);
                    p.ProductCode = 0;
                    p.ClockNumber = string.Empty;
                    p.ProgrammeCategory = string.Empty;
                    p.RestrictionBasis = RestrictionBasis.ClearanceCode;
                    p.RestrictionType = RestrictionType.Time;
                    p.ProgrammeClassification = string.Empty;
                    p.IndexType = 0;
                    p.IndexThreshold = 0;
                }).CreateMany(count);

            return restrictions;
        }           
    }
}
