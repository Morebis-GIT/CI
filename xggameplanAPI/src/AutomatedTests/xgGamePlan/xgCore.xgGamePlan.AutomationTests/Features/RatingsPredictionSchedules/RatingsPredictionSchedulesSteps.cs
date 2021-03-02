using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Demographics;
using xgCore.xgGamePlan.ApiEndPoints.Models.RatingsPredictionSchedules;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.RatingsPredictionSchedules
{
    [Binding]
    public class RatingsPredictionSchedulesSteps : BaseSteps<IRatingsPredictionSchedulesApi>
    {
        public RatingsPredictionSchedulesSteps(ScenarioContext context) : base(context)
        {
        }

        [Given(@"I have added (.*) Ratings Prediction Schedule")]
        public async Task GivenIHaveAddedRatingsPredictionSchedule(int count)
        {
            var ratingsPredictionSchedules = await CreateRatingsPredictionSchedules(count).ConfigureAwait(false);
            ScenarioContext.Set(ratingsPredictionSchedules);
        }

        [When(@"I search for Ratings Prediction Schedules")]
        public async Task WhenISearchForRatingsPredictionSchedules()
        {
            var ratingPredictionSchedule = ScenarioContext.Get<IEnumerable<RatingsPredictionSchedule>>().First();
            var ratingPredictionSchedules = await Api.Search(new RatingsPredictionScheduleSearchModel
            {
                FromScheduleDate = ratingPredictionSchedule.ScheduleDay.ToLocalTime(),
                ToScheduleDate = ratingPredictionSchedule.ScheduleDay.AddDays(1).ToLocalTime(),
                SalesArea = ratingPredictionSchedule.SalesArea
            }).ConfigureAwait(false);
            ScenarioContext.Set(ratingPredictionSchedules);
        }

        [Then(@"at least (.*) Ratings Prediction Schedule is returned")]
        public void ThenAtLeastRatingsPredictionScheduleIsReturned(int count)
        {
            var ratingPredictionSchedules = ScenarioContext.Get<IEnumerable<RatingsPredictionSchedule>>();
            Assert.That(ratingPredictionSchedules.Count() >= count);
        }

        [When(@"I delete all Ratings Prediction Schedules")]
        public async Task WhenIDeleteAllRatingsPredictionSchedules()
        {
            await Api.DeleteAll().ConfigureAwait(false);
        }

        [Then(@"no Ratings Prediction Schedules are returned")]
        public async Task ThenNoRatingsPredictionSchedulesAreReturned()
        {
            var ratingPredictionSchedule = ScenarioContext.Get<IEnumerable<RatingsPredictionSchedule>>().First();
            var ratingPredictionSchedules = await Api.Search(new RatingsPredictionScheduleSearchModel
            {
                FromScheduleDate = DateTime.Now.AddYears(-50),
                ToScheduleDate = DateTime.Now.AddYears(50),
                SalesArea = ratingPredictionSchedule.SalesArea
            }).ConfigureAwait(false);
            Assert.IsEmpty(ratingPredictionSchedules);
        }

        private async Task<IEnumerable<RatingsPredictionSchedule>> CreateRatingsPredictionSchedules(int count)
        {
            var demographic = ScenarioContext.Get<Demographic>();
            var salesArea = ScenarioContext.Get<SalesArea>();

            Fixture.Customize<Rating>(composer => composer
                    .With(p => p.Demographic, demographic.ExternalRef));

            var ratingsPredictionSchedules = Fixture.Build<RatingsPredictionSchedule>()
                .With(p => p.SalesArea, salesArea.Name)
                .With(p => p.ScheduleDay, DateTime.UtcNow)
                .CreateMany(count);

            await Api.Create(ratingsPredictionSchedules).ConfigureAwait(false);

            return ratingsPredictionSchedules;
        }
    }
}
