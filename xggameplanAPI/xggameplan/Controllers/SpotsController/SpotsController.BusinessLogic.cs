using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.AuditEvents;
using xggameplan.common.Services;
using xggameplan.model.External;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    public partial class SpotsController
    {
        /// <summary>
        /// Creates set of spots
        /// </summary>
        /// <param name="spots">entities to be persisted</param>
        /// <returns>List of created entities</returns>
        private IEnumerable<Spot> CreateSpots(List<CreateSpot> spots)
        {
            var createdSpots = new List<Spot>();

            bool isScheduleDataUploadStarted = false;
            using (MachineLock.Create("xggameplan.checkisscheduledatauploadstarted", TimeSpan.FromSeconds(30)))
            {
                isScheduleDataUploadStarted = _repository.CountAll == 0 && _breakRepository.CountAll == 0;
            }

            foreach (var spot in spots)
            {
                var dto = _mapper.Map<Spot>(spot);
                dto.Uid = Guid.NewGuid();
                _repository.Add(dto);

                createdSpots.Add(dto);
            }

            // Generate notification for schedule data upload started
            if (isScheduleDataUploadStarted)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForScheduleDataUploadStarted(0, 0, null));
            }

            return createdSpots;
        }

        /// <summary>
        /// Method checks Spot ExternalBreakNo/StartDateTime/EndDateTime to validate Spot move to the new Break
        /// </summary>
        /// <param name="newSpot" cref="Model.CreateSpot">New entity</param>
        /// <param name="existingSpot" cref="Spot">Existing entity, used in update mode</param>
        /// <returns>Returns result with Success flag and optional ErrorModel if there were some errors</returns>
        private (bool Success, ErrorModel Error) ReviseAndModifySpotChanges(CreateSpot newSpot, Spot existingSpot)
        {
            if (newSpot is null)
            {
                return Failed("", "Entity cannot be null.");
            }

            if (newSpot.EndDateTime != default && newSpot.EndDateTime < newSpot.StartDateTime)
            {
                return Failed(nameof(Spot.EndDateTime), "Spot EndDateTime cannot be less than StartDateTime");
            }

            var spotIsMoved = !newSpot.IsUnplaced && (existingSpot == null ||
                                                       existingSpot.ExternalBreakNo != newSpot.ExternalBreakNo ||
                                                       existingSpot.StartDateTime.Date != newSpot.StartDateTime.Date ||
                                                       existingSpot.SalesArea != newSpot.SalesArea);

            return spotIsMoved ? TryMoveSpotToDifferentBreak(newSpot) : Success();
        }

        private (bool Success, ErrorModel Error) TryMoveSpotToDifferentBreak(CreateSpot newSpot)
        {
            var newBreak = _breakRepository.FindByExternal(newSpot.ExternalBreakNo).FirstOrDefault();

            if (newBreak is null)
            {
                return Failed(nameof(Spot.ExternalBreakNo), $"Break with reference number {newSpot.ExternalBreakNo} could not be found");
            }

            if (newSpot.SalesArea != newBreak.SalesArea)
            {
                return Failed(nameof(Spot.SalesArea), "Spot SalesArea does not match Break SalesArea");
            }

            if (newSpot.StartDateTime.Date != newBreak.ScheduledDate.Date)
            {
                return Failed(nameof(Spot.StartDateTime), "Spot StartDateTime date does not match Break ScheduledDate date");
            }

            return Success();
        }

        /// <summary>
        /// Validate parameters for spots delete endpoint
        /// </summary>
        /// <param name="dateRangeStart"></param>
        /// <param name="dateRangeEnd"></param>
        /// <param name="salesAreaNames"></param>
        /// <returns>Collections of error messages with property names</returns>
        private static IReadOnlyDictionary<string, string> ValidateForDelete(
            DateTime dateRangeStart,
            DateTime dateRangeEnd,
            IReadOnlyCollection<string> salesAreaNames)
        {
            var result = new Dictionary<string, string>();

            if (dateRangeStart == default)
            {
                result.Add(nameof(dateRangeStart), "Date range start is missing");
            }

            if (dateRangeEnd == default)
            {
                result.Add(nameof(dateRangeEnd), "Date range end is missing");
            }

            if (dateRangeStart > dateRangeEnd)
            {
                result.Add("Date range", "Date range start must be before or equal to the date range end");
            }

            if (salesAreaNames?.Any() != true)
            {
                result.Add(nameof(salesAreaNames), "Sales area names are missing");
            }

            return result;
        }

        internal IHttpActionResult DeleteSpotsByDateRangeAndSalesAreas(
            DateTimeRange dateTimeRangeToDelete,
            IReadOnlyCollection<string> salesAreaNames
            )
        {
            IReadOnlyCollection<Break> breaksWithSpotsToDelete = _breakRepository
                .Search(dateTimeRangeToDelete, salesAreaNames)
                .ToList()
                .AsReadOnly();

            if (breaksWithSpotsToDelete.Count == 0)
            {
                return NotFound();
            }

            IReadOnlyCollection<string> externalBreakReferences = breaksWithSpotsToDelete
                .Select(b => b.ExternalBreakRef)
                .ToList()
                .AsReadOnly();

            var spotsToDelete = _repository
                .FindByExternalBreakNumbers(externalBreakReferences)
                .ToList();

            if (spotsToDelete.Count == 0)
            {
                return NotFound();
            }

            _repository.Delete(spotsToDelete.Select(spot => spot.Uid));

            return Ok();
        }

        private static (bool Success, ErrorModel Error) Success() => (true, null);

        private static (bool Success, ErrorModel Error) Failed(string field, string message) =>
            (false, new ErrorModel { ErrorField = field, ErrorMessage = message });
    }
}
