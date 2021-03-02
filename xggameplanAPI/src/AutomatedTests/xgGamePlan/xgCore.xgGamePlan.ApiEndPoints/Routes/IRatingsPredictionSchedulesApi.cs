using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.RatingsPredictionSchedules;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IRatingsPredictionSchedulesApi
    {
        [Get("/RatingsPredictionSchedules/Search")]
        Task<IEnumerable<RatingsPredictionSchedule>> Search(RatingsPredictionScheduleSearchModel ratingsPredictionScheduleSearchModel);

        [Post("/RatingsPredictionSchedules")]
        Task<ApiErrorResult> Create(IEnumerable<RatingsPredictionSchedule> ratingsPredictionSchedules);

        [Delete("/RatingsPredictionSchedules/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
