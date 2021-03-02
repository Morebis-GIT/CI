using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for Ratings Prediction Schedules
    /// </summary>
    [RoutePrefix("RatingsPredictionSchedules")]
    public class RatingsPredictionSchedulesController : ApiController
    {
        private readonly IRatingsScheduleRepository _ratingsScheduleRepository;
        private readonly IDataChangeValidator _dataChangeValidator;
        private readonly IMapper _mapper;

        public RatingsPredictionSchedulesController(
            IRatingsScheduleRepository ratingsScheduleRepository,
            IDataChangeValidator dataChangeValidator,
            IMapper mapper
            )
        {
            _ratingsScheduleRepository = ratingsScheduleRepository;
            _dataChangeValidator = dataChangeValidator;
            _mapper = mapper;
        }

        /// <summary>
        /// Searches RatingsPredictionSchedule documents for schedule date range and sales area
        /// </summary>
        /// <param name="salesArea"></param>
        /// <param name="fromScheduleDate"></param>
        /// <param name="toScheduleDate"></param>
        /// <returns></returns>
        [Route("Search")]
        [AuthorizeRequest("RatingsPredictionSchedules")]
        [ResponseType(typeof(List<RatingsPredictionScheduleModel>))]
        public IHttpActionResult Get([FromUri] DateTime fromScheduleDate,
                                     [FromUri] DateTime toScheduleDate,
                                     [FromUri] string salesArea)
        {
            if (String.IsNullOrEmpty(salesArea) || fromScheduleDate == null || toScheduleDate == null)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }

            var ratingsPredictionSchedules = _ratingsScheduleRepository.GetSchedules(fromScheduleDate.ToUniversalTime(), toScheduleDate.ToUniversalTime(), salesArea);
            var ratingsPredictionScheduleModels = _mapper.Map<List<RatingsPredictionScheduleModel>>(ratingsPredictionSchedules);
            return Ok(ratingsPredictionScheduleModels);
        }

        /// <summary>
        /// Adds RatingsPredictionSchedule documents
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("RatingsPredictionSchedules")]
        public IHttpActionResult Post([FromBody] List<CreateRatingsPredictionSchedule> command)
        {
            if (command == null || !ModelState.IsValid || !command.Any())
            {
                return this.Error().InvalidParameters("Invalid Ratings Prediction Schedule parameters");
            }

            // Create list of RatingsPredictionSchedule instances to add
            var ratingsPredictionSchedules = new List<RatingsPredictionSchedule>();
            foreach (var ratingsPredictionScheduleModel in command)
            {
                var ratingsPredictionSchedule = _ratingsScheduleRepository.GetSchedule(ratingsPredictionScheduleModel.ScheduleDay.Date, ratingsPredictionScheduleModel.SalesArea);
                if (ratingsPredictionSchedule != null)          // Exists already, remove it
                {
                    _ratingsScheduleRepository.Remove(ratingsPredictionSchedule);
                }
                ratingsPredictionSchedules.Add(_mapper.Map<RatingsPredictionSchedule>(ratingsPredictionScheduleModel));
            }

            // Add
            _ratingsScheduleRepository.Insert(ratingsPredictionSchedules);
            return Ok();
        }

        /// <summary>
        /// Delete all RatingsPredictionSchedules
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("RatingsPredictionSchedules")]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<RatingsPredictionSchedule>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _ratingsScheduleRepository.TruncateAsync();

            return Ok();
        }
    }
}
