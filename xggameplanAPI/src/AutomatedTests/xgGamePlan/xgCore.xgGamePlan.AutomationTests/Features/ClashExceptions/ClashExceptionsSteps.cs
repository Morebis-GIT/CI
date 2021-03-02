using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.Clashes;
using xgCore.xgGamePlan.ApiEndPoints.Models.ClashExceptions;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.ClashExceptions
{
    [Binding]
    public class ClashExceptionsSteps : BaseSteps<IClashExceptionsApi>
    {
        private readonly DateTime startClashExceptionDate = DateTime.UtcNow;
        private readonly DateTime endClashExceptionDate = DateTime.UtcNow.AddDays(2);

        public ClashExceptionsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I have removed Clash Exceptions for next two days")]
        public async Task GivenIHaveRemovedClashExceptionsForNextTwoDays()
        {
            //clean up the testing dates for make exceptions available for those days
            await Api.DeleteAllByDate(startClashExceptionDate, endClashExceptionDate).ConfigureAwait(false);
        }

        [Given(@"I know how many Clash Exceptions there are")]
        public async Task GivenIKnowHowManyClashExceptionsThereAre()
        {
            var clashExceptions = await Api.GetAll(new ClashExceptionsGetQueryModel() { Top = 999 }).ConfigureAwait(false) ?? new GetClashExceptionsResult();
            ScenarioContext.Set(clashExceptions.totalCount, "clashExceptionsCount");
        }

        [When(@"I add (.*) Clash Exceptions for next two days")]
        public async Task WhenIAddClashExceptionsForNextTwoDays(int count)
        {
            var clash = ScenarioContext.Get<Clash>();
            await CreateClashExceptions(clash, count, startClashExceptionDate, endClashExceptionDate).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Clash Exceptions are returned")]
        public async Task ThenAdditionalClashExceptionsAreReturned(int additionalClashExceptionsCount)
        {
            var existingClashExceptions = await Api.GetAll(new ClashExceptionsGetQueryModel() { Top = 999 }).ConfigureAwait(false) ?? new GetClashExceptionsResult();
            var previousClashExceptionsCount = ScenarioContext.Get<int>("clashExceptionsCount");

            Assert.That(existingClashExceptions.totalCount == previousClashExceptionsCount + additionalClashExceptionsCount);
        }

        [Given(@"I have added (.*) Clash Exception")]
        public async Task GivenIHaveAddedClashException(int count)
        {
            var clash = ScenarioContext.Get<Clash>();
            var clashException = await CreateClashExceptions(clash, count, startClashExceptionDate, endClashExceptionDate).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(clashException.FirstOrDefault());
        }

        [When(@"I update Clash Exception by ID")]
        public async Task WhenIUpdateClashExceptionByID()
        {
            var newEndDate = DateTime.Now.AddDays(10);
            ScenarioContext.Set(newEndDate, "clashExceptionNewEndDate");
            var clashException = ScenarioContext.Get<ClashException>();
            var updatedClashException = Fixture.Build<ClashExceptionUpdateModel>()
                .With(e => e.EndDate, newEndDate)
                .With(e => e.TimeAndDows, clashException.TimeAndDows)
                .Create();
            await Api.Update(clashException.Id, updatedClashException).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"updated Clash Exception is returned")]
        public async Task ThenUpdatedClashExceptionIsReturned()
        {
            var clashException = ScenarioContext.Get<ClashException>();
            var clashExceptionNewEndDate = ScenarioContext.Get<DateTime>("clashExceptionNewEndDate");
            var existingClashExceptions = await Api.GetAll(
                new ClashExceptionsGetQueryModel()
                {
                    Top = 999,
                    EndDate = clashExceptionNewEndDate
                }).ConfigureAwait(false);

            Assert.IsTrue(existingClashExceptions.items.Where(c => c.Id == clashException.Id).Any());
        }

        [When(@"I remove my Clash Exception by ID")]
        public async Task WhenIRemoveMyClashExceptionByID()
        {
            var clashException = ScenarioContext.Get<ClashException>();
            await Api.Delete(clashException.Id).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Clash Exception with ID is returned")]
        public async Task ThenNoClashExceptionWithIDIsReturned()
        {
            var clashException = ScenarioContext.Get<ClashException>();
            var existingClashExceptions = await Api.GetAll(
                new ClashExceptionsGetQueryModel()
                {
                    Top = 999
                }).ConfigureAwait(false);
            //When Clash Exception is not found server returns 401 error, refit have only general ApiExeption type
            var response = await Api.GetByIdResponse(clashException.Id).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Wrong expected status code.");
        }

        private async Task<List<ClashException>> CreateClashExceptions(Clash clash, int count, DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;
            var timesAndDows = Fixture.Build<TimeAndDow>()
                .With(tad => tad.DaysOfWeek, "1111111")
                .With(tad => tad.StartTime, now.AddMilliseconds(-5).TimeOfDay)
                .With(tad => tad.EndTime, now.TimeOfDay)
                .CreateMany(count);
            Fixture.Customize<ClashException>(c => c.With(x => x.TimeAndDows, timesAndDows));
            var newClashExceptions = Fixture.Build<ClashException>()
                .With(c => c.StartDate, startDate)
                .With(c => c.EndDate, endDate)
                .With(c => c.TimeAndDows, timesAndDows)
                .With(c => c.FromValue, clash.Externalref)
                .With(c => c.ToValue, clash.Externalref)
                .With(c => c.ToType, ClashExceptionType.Clash)
                .With(c => c.FromType, ClashExceptionType.Clash)
                .With(c => c.IncludeOrExclude, IncludeOrExclude.E)
                .CreateMany(count);            
            return await Api.Create(newClashExceptions).ConfigureAwait(false);
        }
    }
}
