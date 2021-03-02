using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using Raven.Abstractions.Extensions;
using xggameplan.AuditEvents;
using xggameplan.common.Services;
using xggameplan.core.Interfaces;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("breaks")]
    public partial class BreaksController : ApiController
    {
        private readonly IBreakRepository _breakRepository;
        private readonly ISpotRepository _spotRepository;
        private readonly IMapper _mapper;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IDataChangeValidator _dataChangeValidator;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IRecalculateBreakAvailabilityService _recalculateBreakAvailabilityService;

        public BreaksController(IBreakRepository breakRepository, IMapper mapper,
            IScheduleRepository scheduleRepository, ISpotRepository spotRepository,
            IDataChangeValidator dataChangeValidator, IAuditEventRepository auditEventRepository,
            ISalesAreaRepository salesAreaRepository,
            IRecalculateBreakAvailabilityService recalculateBreakAvailabilityService)
        {
            _breakRepository = breakRepository;
            _spotRepository = spotRepository;
            _mapper = mapper;
            _scheduleRepository = scheduleRepository;
            _dataChangeValidator = dataChangeValidator;
            _auditEventRepository = auditEventRepository;
            _salesAreaRepository = salesAreaRepository;
            _recalculateBreakAvailabilityService = recalculateBreakAvailabilityService;
        }

        /// <summary>
        /// Get all Breaks Never going to production! just for testing
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Breaks")]
        [ResponseType(typeof(IEnumerable<BreakModel>))]
        public IEnumerable<BreakModel> Get()
        {
            var breaks = _breakRepository.GetAll();
            if (breaks != null && breaks.Any())
            {
                return _mapper.Map<List<BreakModel>>(breaks.ToList());
            }

            return null;
        }

        /// <summary>
        /// Gets a single Break
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [AuthorizeRequest("Breaks")]
        [ResponseType(typeof(BreakModel))]
        public IHttpActionResult Get(Guid id)
        {
            var item = _breakRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BreakModel>(item));
        }

        /// <summary>
        /// Creates a set of Breaks
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Breaks")]
        public IHttpActionResult Post([FromBody] List<CreateBreak> breaks)
        {
            if (breaks is null || !breaks.Any() || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Determine if they've just started uploading schedule data
            bool isScheduleDataUploadStarted = false;

            using (MachineLock.Create("xggameplan.checkisscheduledatauploadstarted", TimeSpan.FromSeconds(30)))
            {
                isScheduleDataUploadStarted = _breakRepository.CountAll == 0 && _spotRepository.CountAll == 0;
            }

            ValidateBreaks(breaks);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // zero out BroadcastDate's times
            foreach (var b in breaks)
            {
                if (b.BroadcastDate.HasValue)
                {
                    b.BroadcastDate = b.BroadcastDate.Value.Date;
                }
            }

            // group by date and channels
            breaks.GroupBy(s => new { s.ScheduledDate.Date, s.SalesArea }).ForEach(grp =>
              {
                  using (MachineLock.Create(string.Format("xggameplan.scheduleday.{0}.{1}", grp.Key.SalesArea, grp.Key.Date), new TimeSpan(0, 10, 0)))
                  {
                      var schedule = _scheduleRepository.GetOrCreateSchedule(grp.Key.SalesArea, grp.Key.Date);
                      var breaklist = _mapper.Map<List<Break>>(grp.ToList());

                      LoadBreakProperties(ref breaklist);

                      _breakRepository.Add(breaklist);

                      schedule.Breaks = breaklist.ToList();
                      _scheduleRepository.Add(schedule);
                      _scheduleRepository.SaveChanges();
                  }
              });

            // Generate notification for schedule data upload started
            if (isScheduleDataUploadStarted)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForScheduleDataUploadStarted(0, 0, null));
            }

            return Ok();
        }

        private void ValidateBreaks(List<CreateBreak> breaks)
        {
            foreach (var b in breaks)
            {
                try
                {
                    b.Validate();
                }
                catch (BreakTypeException ex)
                {
                    ModelState.AddModelError(
                        nameof(CreateBreak.BreakType), BreakTypeValueErrorMessage(ex)
                        );
                }
            }

            _salesAreaRepository.ValidateSaleArea(breaks.Select(s => s.SalesArea).ToList());

            // Local functions
            string BreakTypeValueErrorMessage(Exception ex)
            {
                const string FieldName = nameof(CreateBreak.ExternalBreakRef);

                var errorMessage = new StringBuilder(200);

                errorMessage.Append(ex.Message);
                errorMessage.Append(" ");
                errorMessage.Append(FieldName);
                errorMessage.Append(" :: ");
                errorMessage.Append(ex.Data[FieldName]);

                return errorMessage.ToString();
            }
        }

        private static void LoadBreakProperties(ref List<Break> breaks)
        {
            var breakid = 0;
            breaks.ForEach(b =>
            {
                b.CustomId = ++breakid;
                b.Id = Guid.NewGuid();
                b.Avail = b.Duration;       // We assume that spots will be uploaded after breaks
                b.OptimizerAvail = b.Duration;
            });
        }

        /// <summary>
        /// Deletes a range of Breaks based on scheduled date
        /// </summary>
        /// <param name="dateRangeStart">Scheduled date</param>
        /// <param name="dateRangeEnd">Scheduled date</param>
        /// <param name="salesAreaNames"></param>
        /// <returns></returns>
        [Route("")]
        [Route("by-scheduled-date")]
        [AuthorizeRequest("Breaks")]
        [HttpDelete]
        public IHttpActionResult Delete([FromUri] DateTime dateRangeStart,
            [FromUri] DateTime dateRangeEnd,
            [FromUri] List<string> salesAreaNames)
        {
            var messages = ValidateForDelete(dateRangeStart, dateRangeEnd, salesAreaNames);

            foreach (var message in messages)
            {
                ModelState.AddModelError(message.Key, message.Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // broadcast day
            var defaultBroadcastStartTime = new TimeSpan(6, 0, 0);
            var defaultBroadcastEndTime = defaultBroadcastStartTime.Subtract(new TimeSpan(0, 0, 1));

            var broadcastStartDateTime = dateRangeStart.Date.Add(defaultBroadcastStartTime);
            var broadcastEndDateTime = dateRangeEnd.Date.AddDays(1).Add(defaultBroadcastEndTime);
            var scheduledDatesRange = new DateTimeRange(broadcastStartDateTime, broadcastEndDateTime);

            var breaks = _breakRepository.Search(scheduledDatesRange, salesAreaNames).ToList();

            if (!breaks.Any())
            {
                return NotFound();
            }

            var breakIdsToDelete = new HashSet<Guid>();
            var breakExtRefsToDelete = new List<string>();

            foreach (var oneBreak in breaks)
            {
                breakIdsToDelete.Add(oneBreak.Id);
                breakExtRefsToDelete.Add(oneBreak.ExternalBreakRef);
            }

            // load spots and schedules to be updated
            var spotsToDelete = _spotRepository.FindByExternalBreakNumbers(breakExtRefsToDelete);
            var schedulesToUpdate = _scheduleRepository.FindByBreakIds(breakIdsToDelete);

            // delete spots first and persist changes
            if (spotsToDelete.Any())
            {
                _spotRepository.Delete(spotsToDelete.Select(s => s.Uid));
                _spotRepository.SaveChanges();
            }

            _breakRepository.Delete(breakIdsToDelete.ToList());

            foreach (var schedule in schedulesToUpdate)
            {
                schedule.Breaks = schedule.Breaks.Where(b => !breakIdsToDelete.Contains(b.Id)).ToList();
                _scheduleRepository.Update(schedule);
                _scheduleRepository.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// Deletes a range of Breaks by Broadcast Date Range
        /// </summary>
        /// <param name="dateRangeStart">Start of Broadcast Date Range</param>
        /// <param name="dateRangeEnd">End of Broadcast Date Range</param>
        /// <param name="salesAreaNames"></param>
        /// <returns></returns>
        [Route("by-broadcast-date")]
        [HttpDelete]
        [AuthorizeRequest("Breaks")]
        public IHttpActionResult DeleteByBroadcastDate([FromUri] DateTime dateRangeStart,
            [FromUri] DateTime dateRangeEnd, [FromUri] List<string> salesAreaNames)
        {
            var messages = ValidateForDelete(dateRangeStart, dateRangeEnd, salesAreaNames);

            foreach (var message in messages)
            {
                ModelState.AddModelError(message.Key, message.Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // zero out times
            var broadcastDateRangeStart = dateRangeStart.Date;
            var broadcastDateRangeEnd = dateRangeEnd.Date;

            var breaks = _breakRepository.SearchByBroadcastDateRange(broadcastDateRangeStart, broadcastDateRangeEnd, salesAreaNames).ToList();

            if (!breaks.Any())
            {
                return this.Error().ResourceNotFound();
            }

            var breakIdsToDelete = new HashSet<Guid>();
            var breakExtRefsToDelete = new List<string>();

            foreach (var oneBreak in breaks)
            {
                breakIdsToDelete.Add(oneBreak.Id);
                breakExtRefsToDelete.Add(oneBreak.ExternalBreakRef);
            }

            // load spots and schedules to be updated
            var spotsToDelete = _spotRepository.FindByExternalBreakNumbers(breakExtRefsToDelete);
            var schedulesToUpdate = _scheduleRepository.FindByBreakIds(breakIdsToDelete);

            // delete spots first and persist changes
            if (spotsToDelete.Any())
            {
                _spotRepository.Delete(spotsToDelete.Select(s => s.Uid));
                _spotRepository.SaveChanges();
            }

            _breakRepository.Delete(breakIdsToDelete.ToList());

            foreach (var schedule in schedulesToUpdate)
            {
                schedule.Breaks = schedule.Breaks.Where(b => !breakIdsToDelete.Contains(b.Id)).ToList();
                _scheduleRepository.Update(schedule);
                _scheduleRepository.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// Delete all Breaks
        /// </summary>
        [Route("DeleteAll")]
        [Route("all")]
        [AuthorizeRequest("Breaks")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Break>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _breakRepository.TruncateAsync();

            return Ok();
        }

        /// <summary>
        /// Validates parameters for break delete endpoint
        /// </summary>
        /// <param name="dateRangeStart"></param>
        /// <param name="dateRangeEnd"></param>
        /// <param name="salesAreaNames"></param>
        /// <returns>Collections of error messages with property names</returns>
        private static Dictionary<string, string> ValidateForDelete(DateTime dateRangeStart, DateTime dateRangeEnd, List<string> salesAreaNames)
        {
            var messages = new Dictionary<string, string>();

            if (dateRangeStart == default)
            {
                messages.Add(nameof(dateRangeStart), "Date range start is missing");
            }

            if (dateRangeEnd == default)
            {
                messages.Add(nameof(dateRangeEnd), "Date range end is missing");
            }

            if (dateRangeStart > dateRangeEnd)
            {
                messages.Add("Date range", "Date range start must be before or equal to the date range end");
            }

            if (salesAreaNames is null || !salesAreaNames.Any())
            {
                messages.Add(nameof(salesAreaNames), "Sales area names are missing");
            }

            return messages;
        }
    }
}
