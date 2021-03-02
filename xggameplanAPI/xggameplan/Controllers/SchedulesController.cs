using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("Schedules")]
    public class SchedulesController : ApiController
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;
        private readonly IDataChangeValidator _dataChangeValidator;

        public SchedulesController(IScheduleRepository scheduleRepository, IMapper mapper, IDataChangeValidator dataChangeValidator)
        {
            _scheduleRepository = scheduleRepository;
            _dataChangeValidator = dataChangeValidator;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all schedules
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Schedules")]
        [ResponseType(typeof(List<ScheduleModel>))]
        public IHttpActionResult Get()
        {
            var schedules = _scheduleRepository.GetAll();
            if (schedules == null || !schedules.Any())
            {
                return this.NoContent();
            }

            return Ok(_mapper.Map<List<ScheduleModel>>(schedules.ToList()));
        }

        /// <summary>
        /// Search and get a range of breaks within the schedules
        /// </summary>
        /// <param name="dateFrom">From date in range</param>
        /// <param name="dateTo">To date in range</param>
        /// <param name="salesAreaNames">Sale area names</param>
        /// <returns>Break List</returns>
        [Route("Break")]
        [AuthorizeRequest("Schedules")]
        [ResponseType(typeof(List<BreakModel>))]
        public IHttpActionResult GetBreaks([FromUri] DateTime dateFrom,
            [FromUri] DateTime dateTo,
            [FromUri] List<string> salesAreaNames)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("body parameters missing");
            }
            dateFrom = dateFrom.ToUniversalTime();
            dateTo = dateTo.ToUniversalTime();
            var breaks = _scheduleRepository.GetBreaks(salesAreaNames, dateFrom, dateTo);
            if (breaks == null || !breaks.Any())
            {
                return this.NoContent();
            }

            var filteredBreaks = breaks.Where(b => b.ScheduledDate >= dateFrom && b.ScheduledDate <= dateTo)
                .ToList();
            return Ok(_mapper.Map<List<BreakModel>>(filteredBreaks));
        }

        /// <summary>
        /// Search and get a range of programmes within the schedules
        /// </summary>
        /// <param name="dateFrom">From date in range</param>
        /// <param name="dateTo">To date in range</param>
        /// <param name="salesAreaNames">Sales area name list</param>
        /// <returns>Programme List</returns>
        [Route("Programme")]
        [AuthorizeRequest("Schedules")]
        [ResponseType(typeof(List<ProgrammeModel>))]
        public IHttpActionResult GetProgrammes([FromUri] DateTime dateFrom,
            [FromUri] DateTime dateTo,
            [FromUri] List<string> salesAreaNames)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("body parameters missing");
            }
            dateFrom = dateFrom.ToUniversalTime();
            dateTo = dateTo.ToUniversalTime();
            var programmes = _scheduleRepository.GetProgrammes(salesAreaNames, dateFrom, dateTo);
            if (programmes == null || !programmes.Any())
            {
                return this.NoContent();
            }

            var filteredProgrammes = programmes.Where(p => p.StartDateTime >= dateFrom && p.StartDateTime <= dateTo)
                .ToList();
            return Ok(_mapper.Map<List<ProgrammeModel>>(filteredProgrammes));
        }

        /// <summary>
        /// Search and get a range of schedules
        /// </summary>
        /// <param name="dateFrom">From date range</param>
        /// <param name="dateTo">To date range</param>
        /// <param name="salesAreaNames">Sales area name list</param>
        /// <returns>Schedule List</returns>
        [Route("Search")]
        [AuthorizeRequest("Schedules")]
        [ResponseType(typeof(List<ScheduleModel>))]
        public IHttpActionResult GetSchedules([FromUri] DateTime dateFrom,
            [FromUri] DateTime dateTo,
            [FromUri] List<string> salesAreaNames)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("body parameters missing");
            }
            dateFrom = dateFrom.ToUniversalTime();
            dateTo = dateTo.ToUniversalTime();
            var schedule = _scheduleRepository.GetSchedule(salesAreaNames, dateFrom, dateTo);
            if (schedule != null && schedule.Any())
            {
                return Ok(_mapper.Map<List<ScheduleModel>>(schedule.ToList()));
                // return Ok((schedule.ToList()));
            }
            return this.NoContent();
        }

        [Route("{id}")]
        [AuthorizeRequest("Schedules")]
        public IHttpActionResult Delete(Guid id)
        {
            return this.NoContent();
        }

        /// <summary>
        /// Delete all schedules
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("Schedules")]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Schedule>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _scheduleRepository.TruncateAsync();

            return Ok();
        }
    }
}
