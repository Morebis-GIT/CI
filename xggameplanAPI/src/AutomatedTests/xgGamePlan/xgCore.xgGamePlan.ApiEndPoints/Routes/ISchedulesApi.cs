using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Breaks;
using xgCore.xgGamePlan.ApiEndPoints.Models.Programmes;
using xgCore.xgGamePlan.ApiEndPoints.Models.Schedules;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ISchedulesApi
    {
        [Get("/Schedules")]
        Task<IEnumerable<Schedule>> GetAll();

        [Get("/Schedules/Programme")]
        Task<IEnumerable<Programme>> GetProgrammes([Query] DateTime dateFrom, [Query] DateTime dateTo, [Query(CollectionFormat.Multi)] IEnumerable<string> salesAreaNames);

        [Get("/Schedules/Break")]
        Task<IEnumerable<Break>> GetBreaks([Query] DateTime dateFrom, [Query] DateTime dateTo, [Query(CollectionFormat.Multi)] IEnumerable<string> salesAreaNames);

        [Delete("/Schedules/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
