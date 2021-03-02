using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using xggameplan.AuditEvents;
using xggameplan.core.Interfaces;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("Spots")]
    public partial class SpotsController : ApiController
    {
        private readonly ISpotRepository _repository;
        private readonly IMapper _mapper;
        private readonly IBreakRepository _breakRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IDataChangeValidator _dataChangeValidator;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly ISpotModelCreator _spotModelCreator;

        public SpotsController(
            ISpotRepository repository,
            IBreakRepository breakRepository,
            IScheduleRepository scheduleRepository,
            IDataChangeValidator dataChangeValidator,
            IAuditEventRepository auditEventRepository,
            ISpotModelCreator spotModelCreator,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _breakRepository = breakRepository;
            _scheduleRepository = scheduleRepository;
            _dataChangeValidator = dataChangeValidator;
            _auditEventRepository = auditEventRepository;
            _spotModelCreator = spotModelCreator;
        }

        /// <summary>
        /// Get all Spots.
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Spots")]
        [ResponseType(typeof(IEnumerable<Spot>))]
        public IEnumerable<Spot> Get()
        {
            var spots = _repository.GetAll();
            return spots;
        }

        /// <summary>
        /// Gets a single Spot
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [AuthorizeRequest("Spots")]
        [ResponseType(typeof(Spot))]
        public IHttpActionResult Get(Guid id)
        {
            var item = _repository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets a set of spots for a associated Campaign Ref
        /// </summary>
        [Route("campaignRef/{campaignRef}")]
        [AuthorizeRequest("Spots")]
        [ResponseType(typeof(List<Spot>))]
        public IHttpActionResult Get(string campaignRef)
        {
            var item = _repository.FindByExternal(campaignRef);

            var spots = item as IList<Spot> ?? item.ToList();
            if (!spots.Any())
            {
                return NotFound();
            }
            return Ok(spots);
        }

        /// <summary>
        /// Gets a range of Spots
        /// </summary>
        /// <param name="datefrom"></param>
        /// <param name="dateto"></param>
        /// <param name="salesarea"></param>
        [Route("Search")]
        [AuthorizeRequest("Spots")]
        [ResponseType(typeof(IEnumerable<SpotModel>))]
        public IHttpActionResult Get([FromUri] DateTime datefrom,
                                     [FromUri] DateTime dateto,
                                     [FromUri] string salesarea)
        {
            datefrom = datefrom.ToUniversalTime();
            dateto = dateto.ToUniversalTime();
            var items = _repository.Search(datefrom, dateto, salesarea);
            return Ok(_spotModelCreator.Create(items.ToList()));
        }

        /// <summary>
        /// Create a set of Spots.
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Spots")]
        public IHttpActionResult Post([FromBody] List<CreateSpot> spots)
        {
            if (spots is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var spot in spots)
            {
                try
                {
                    spot.Validate();
                }
                catch (BreakTypeException ex)
                {
                    ModelState.AddModelError(
                        nameof(CreateSpot.BreakType), BreakTypeValueErrorMessage(ex)
                        );
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CreateSpots(spots);

            _repository.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Update spot
        /// </summary>
        /// <param name="id"></param>
        /// <param name="spot"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Spots")]
        public IHttpActionResult Put(Guid id, [FromBody] CreateSpot spot)
        {
            if (id == default(Guid) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingSpot = _repository.Find(id);
            if (existingSpot is null)
            {
                return this.Error().ResourceNotFound("Spot not found");
            }

            try
            {
                spot.Validate();
            }
            catch (BreakTypeException ex)
            {
                ModelState.AddModelError(
                    nameof(CreateSpot.BreakType), BreakTypeValueErrorMessage(ex)
                    );

                return BadRequest(ModelState);
            }

            existingSpot.Update(spot);
            _repository.Update(existingSpot);

            _repository.SaveChanges();

            return Ok(existingSpot);
        }

        /// <summary>
        /// Update (or create) spot by externalId
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="newSpot"></param>
        /// <returns>
        /// <para>Http Status Code 200 - Spot was successfuly update</para>
        /// <para>
        /// Http Status Code 400 - Payload is incorrect or spot with provided
        /// external reference was not found
        /// </para>
        /// <para>
        /// Http Status Code 412 - Spot is updated, however placing conditions
        /// where not met, spot is moved to unplaced list.
        /// </para>
        /// </returns>
        [Route("externalref/{externalId}")]
        [AuthorizeRequest("Spots")]
        [ResponseType(typeof(SpotModel))]
        public IHttpActionResult Put(string externalId, [FromBody] CreateSpot newSpot)
        {
            if (newSpot != null && (string.IsNullOrWhiteSpace(externalId) || externalId != newSpot.ExternalSpotRef))
            {
                ModelState.AddModelError(nameof(Spot.ExternalSpotRef), "External Id does not match");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                newSpot.Validate();
            }
            catch (BreakTypeException ex)
            {
                ModelState.AddModelError(
                    nameof(CreateSpot.BreakType), BreakTypeValueErrorMessage(ex)
                    );

                return BadRequest(ModelState);
            }

            var existingSpot = _repository.FindByExternalSpotRef(externalId);
            var reviseResult = ReviseAndModifySpotChanges(newSpot, existingSpot);

            if (!reviseResult.Success)
            {
                ModelState.AddModelError(reviseResult.Error.ErrorField, reviseResult.Error.ErrorMessage);
                return BadRequest(ModelState);
            }

            if (existingSpot is null)
            {
                existingSpot = CreateSpots(new List<CreateSpot> { newSpot }).First();
            }
            else
            {
                existingSpot.Update(newSpot);
                _repository.Update(existingSpot);
            }

            // Do not remove this, need to persist changes now so that we can
            // return model
            _repository.SaveChanges();

            return Ok(_spotModelCreator.Create(new[] { existingSpot }).FirstOrDefault());
        }

        private string BreakTypeValueErrorMessage(Exception ex)
        {
            var errorMessage = new StringBuilder(200);

            errorMessage.Append(ex.Message);
            errorMessage.Append(" ");
            errorMessage.Append(nameof(CreateSpot.ExternalSpotRef));
            errorMessage.Append(" :: ");
            errorMessage.Append(ex.Data[nameof(CreateSpot.ExternalSpotRef)]);

            return errorMessage.ToString();
        }

        /// <summary>
        /// Deletes a single Spot
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [AuthorizeRequest("Spots")]
        public IHttpActionResult Delete(Guid id)
        {
            var spot = _repository.Find(id);
            if (spot is null)
            {
                return this.Error().ResourceNotFound("Spot not found");
            }

            _repository.Remove(id);
            return Ok();
        }

        /// <summary>
        /// Delete a range of Spots booked into breaks between two dates in the
        /// given sales area(s).
        /// </summary>
        /// <param name="dateRangeStart">
        /// Start datetime of the breaks in UTC format.
        /// </param>
        /// <param name="dateRangeEnd">End datetime of the breaks in UTC format.</param>
        /// <param name="salesAreaNames">
        /// Breaks are searched in this list of sales area names.
        /// </param>
        /// <returns>Returns a response code indicating success or otherwise.</returns>
        [Route("")]
        [AuthorizeRequest("Spots")]
        [HttpDelete]
        public IHttpActionResult Delete(
            [FromUri] DateTime dateRangeStart,
            [FromUri] DateTime dateRangeEnd,
            [FromUri] List<string> salesAreaNames
            )
        {
            var cleanSalesAreaNames = salesAreaNames?
                .Where(name => !String.IsNullOrWhiteSpace(name))
                .ToList()
                .AsReadOnly();

            IReadOnlyDictionary<string, string> validationErrors = ValidateForDelete(
                dateRangeStart,
                dateRangeEnd,
                cleanSalesAreaNames);

            AddValidationErrorsToModelErrors(validationErrors);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return DeleteSpotsByDateRangeAndSalesAreas(
                (dateRangeStart, dateRangeEnd),
                cleanSalesAreaNames
                );
        }

        private void AddValidationErrorsToModelErrors(
            IReadOnlyDictionary<string, string> validationErrors
            )
        {
            if (validationErrors.Count == 0)
            {
                return;
            }

            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        /// <summary>
        /// Delete a range of Spots by external spot references
        /// </summary>
        /// <param name="externalSpotRef"></param>
        /// <returns></returns>
        [Route("externalref")]
        [AuthorizeRequest("Spots")]
        public IHttpActionResult Delete([FromUri] List<string> externalSpotRef)
        {
            externalSpotRef = externalSpotRef?.Where(name => !string.IsNullOrWhiteSpace(name)).ToList();
            if (externalSpotRef == null || !externalSpotRef.Any())
            {
                ModelState.AddModelError(nameof(externalSpotRef), "External spot references are missing");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var spots = _repository.FindByExternal(externalSpotRef).ToList();

            if (spots.Count == 0)
            {
                return NotFound();
            }

            var multipartSpots = new Lazy<List<Spot>>(() => _repository.GetAllMultipart().ToList());
            var spotsToDelete = new List<Guid>();

            foreach (var spot in spots)
            {
                if (!spot.IsMultipartSpot)
                {
                    spotsToDelete.Add(spot.Uid);
                }
                else if (spot.IsTopMultipart())
                {
                    spotsToDelete.AddRange(
                        BreakUtilities.GetLinkedMultipartSpots(spot, multipartSpots.Value, true)
                            .Select(s => s.Uid)
                        );
                }
            }

            _repository.Delete(spotsToDelete.Distinct());

            return Ok();
        }

        /// <summary>
        /// Deletes all spots
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("Spots")]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Spot>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _repository.TruncateAsync();

            return Ok();
        }

        /// <summary>
        /// Gets a range of Spots with break and programme details
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="salesarea"></param>
        [Route("BreakAndProgrammeInfo")]
        [AuthorizeRequest("Spots")]
        [ResponseType(typeof(List<SpotWithBreakAndProgrammeInfo>))]
        public IHttpActionResult GetSpot([FromUri] DateTime dateFrom,
            [FromUri] DateTime dateTo,
            [FromUri] string salesarea)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Parameters Invalid");
            }
            dateFrom = dateFrom.ToUniversalTime();
            dateTo = dateTo.ToUniversalTime();

            var breaksAndProgrammes =
                _scheduleRepository.GetBreakWithProgramme(new List<string>() { salesarea }, dateFrom, dateTo);
            if (breaksAndProgrammes != null)
            {
                var spots = _repository.Search(dateFrom, dateTo, salesarea)?.ToList();
                List<SpotModel> spotModels = new List<SpotModel>();
                if (spots != null && spots.Any())
                {
                    spotModels = _spotModelCreator.Create(spots).ToList();
                }
                return Ok(_mapper.Map<List<SpotWithBreakAndProgrammeInfo>>(Tuple.Create(spotModels,
                    breaksAndProgrammes)));
            }

            return NotFound();
        }
    }
}
