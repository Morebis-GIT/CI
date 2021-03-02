using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations;

namespace xggameplan.Controllers
{
    /// <summary>
    ///  Sponsorship Controller
    /// </summary>
    [RoutePrefix("Sponsorships")]
    public class SponsorshipsController : ApiController
    {
        private const string AuthorizeRequestActionName = "Sponsorships";     
        private readonly IMapper _mapper;        
        private readonly IModelDataValidator<IEnumerable<CreateSponsorshipModel>> _createSponsorshipsValidator;
        private readonly IModelDataValidator<UpdateSponsorshipModel> _updateSponsorshipValidator;
        private readonly ISponsorshipRepository _sponsorshipRepository;
        private readonly IDataChangeValidator _dataChangeValidator;

        public SponsorshipsController(
            ISponsorshipRepository sponsorshipRepository,
            IDataChangeValidator dataChangeValidator,
            IMapper mapper,
            IModelDataValidator<IEnumerable<CreateSponsorshipModel>> createSponsorshipsValidator,
            IModelDataValidator<UpdateSponsorshipModel> updateSponsorshipValidator
            )
        {
            _sponsorshipRepository = sponsorshipRepository;
            _dataChangeValidator = dataChangeValidator;
            _mapper = mapper;            
            _createSponsorshipsValidator = createSponsorshipsValidator;
            _updateSponsorshipValidator = updateSponsorshipValidator;
        }

        /// <summary>
        /// Get all Sponsorships.
        /// </summary>
        [Route("")]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        public IEnumerable<SponsorshipModel> Get()
        {
            IEnumerable<Sponsorship> sponsorships = _sponsorshipRepository.GetAll();
            return Mappings.MapToSponsorshipModels(sponsorships, _mapper);
        }

        /// <summary>
        /// Creates one or more Sponsorship items based on the supplied createSponsorshipModels
        /// </summary>
        /// <param name="createSponsorshipModels" type="IEnumerable{CreateSponsorshipModel}">The collection of CreateSponsorshipModel</param>
        /// <returns>200 OK response if successful</returns>
        /// <response code="200">Returns 200 OK response</response>
        /// <response code="400">Returns 400 Bad Request, when the supplied model is invalid/fails validation</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [Route("")]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        public IHttpActionResult Post([FromBody] IEnumerable<CreateSponsorshipModel> createSponsorshipModels)
        {
            if (createSponsorshipModels == null || !createSponsorshipModels.Any() || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_createSponsorshipsValidator.IsValid(createSponsorshipModels))
            {
                return _createSponsorshipsValidator.BadRequest();
            }

            foreach (var sponsorship in createSponsorshipModels.Select(s => _mapper.Map<Sponsorship>(s)))
            {
                sponsorship.DateCreated = DateTime.UtcNow;
                sponsorship.DateModified = DateTime.UtcNow;
                sponsorship.Uid = Guid.NewGuid();
                _sponsorshipRepository.Add(sponsorship);
            }

            _sponsorshipRepository.SaveChanges();

            return Ok();
        }        

        /// <summary>
        /// Delete a single Sponsorship using the supplied externalReferenceId.
        /// </summary>
        /// <param name = "externalReferenceId">Sponsorship externalReferenceId.</param>
        /// <returns> Status Code 204 NoContent Result if successfull.</returns>
        /// <response code="204">Returns NoContent if Deletion is successfull.</response>
        /// <response code="400">Returns BadRequest if externalReferenceId is invalid.</response>
        /// <response code="404">Returns NotFound if no Sponsorship found.</response>
        /// <response code="500">Returns Internal Server Error for undefined failure.</response>
        [HttpDelete]
        [Route("{externalReferenceId}")]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        public IHttpActionResult Delete(string externalReferenceId)
        {
            if (string.IsNullOrWhiteSpace(externalReferenceId))
            {
                return BadRequest("Invalid Sponsorship externalReferenceId");
            }

            if (_sponsorshipRepository.Get(externalReferenceId) == null)
            {
                return NotFound();
            }

            _sponsorshipRepository.Delete(externalReferenceId);
            _sponsorshipRepository.SaveChanges();

            return this.NoContent();
        }

        /// <summary>
        /// Delete all Sponsorships
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        public async Task<IHttpActionResult> DeleteAll()
        {
            //Validate that we can delete(is run scheduled or active)
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Sponsorship>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _sponsorshipRepository.TruncateAsync().ConfigureAwait(false);

            return Ok();
        }

        /// <summary>
        /// Upates a Sponsorship item based on the supplied updateSponsorshipModel
        /// </summary>
        /// <param name="updateSponsorshipModel" type="{UpdateSponsorshipModel}">The updateSponsorshipModel</param>
        /// <returns>200 OK response if update is successful</returns>
        /// <response code="200">Returns 200 OK response</response>
        /// <response code="400">Returns 400 Bad Request, when the supplied model is invalid/fails validation</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [HttpPut]
        [Route("")]
        [AuthorizeRequest("AuthorizeRequestActionName")]
        public IHttpActionResult Put([FromBody] UpdateSponsorshipModel updateSponsorshipModel)
        {
            if (updateSponsorshipModel == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_updateSponsorshipValidator.IsValid(updateSponsorshipModel))
            {
                return _updateSponsorshipValidator.BadRequest();
            }

            Sponsorship existingsponsorshipitem = _sponsorshipRepository.Get(updateSponsorshipModel.ExternalReferenceId);
            if (existingsponsorshipitem == null)
            {
                return NotFound();
            }

            UpdateSingleSponsorship(updateSponsorshipModel, existingsponsorshipitem);

            return Ok();
        }

        private void UpdateSingleSponsorship(CreateSponsorshipModel modifiedsponsorshipitem, Sponsorship existingsponsorshipitem)
        {
            existingsponsorshipitem.DateModified = DateTime.UtcNow;
            existingsponsorshipitem.RestrictionLevel = modifiedsponsorshipitem.RestrictionLevel;

            existingsponsorshipitem.SponsoredItems.Clear();

            foreach (CreateSponsoredItemModel item in modifiedsponsorshipitem.SponsoredItems)
            {
                existingsponsorshipitem.SponsoredItems.Add(_mapper.Map<SponsoredItem>(item));
            }

            _sponsorshipRepository.Update(existingsponsorshipitem);

            _sponsorshipRepository.SaveChanges();
        }
    }
}
