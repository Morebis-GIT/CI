using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using xggameplan.core.Helpers;
using xggameplan.core.Validators;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("Clash")]
    public partial class ClashController : ApiController
    {
        private readonly IClashRepository _clashRepository;
        private readonly IDataChangeValidator _dataChangeValidator;
        private readonly IMapper _mapper;
        private readonly IClashValidator _clashValidator;

        public ClashController(
            IClashRepository clashRepository,
            IDataChangeValidator dataChangeValidator,
            IMapper mapper,
            IClashValidator clashValidator
            )
        {
            _clashRepository = clashRepository;
            _dataChangeValidator = dataChangeValidator;
            _mapper = mapper;
            _clashValidator = clashValidator;
        }

        /// <summary>
        /// Get all Clash
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Clash")]
        [ResponseType(typeof(IEnumerable<ClashModel>))]
        public IEnumerable<ClashModel> Get()
        {
            var clashes = _clashRepository.GetAll()?.ToList();
            if (clashes != null && clashes.Any())
            {
                return clashes.Select(clash =>
               {
                   var parentclash = string.IsNullOrWhiteSpace(clash.ParentExternalidentifier)
                       ? null
                       : clashes.FirstOrDefault(c => c.Externalref.Equals(clash.ParentExternalidentifier,
                           StringComparison.OrdinalIgnoreCase));

                   return _mapper.Map<ClashModel>(Tuple.Create(clash, parentclash));
               }).ToList();
            }
            return null;
        }

        /// <summary>
        /// Gets a clash
        /// </summary>
        /// <param name = "id" ></param >
        [Route("{id}")]
        [AuthorizeRequest("Clash")]
        [ResponseType(typeof(ClashModel))]
        public IHttpActionResult Get(Guid id)
        {
            var item = _clashRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            var parentclash = string.IsNullOrWhiteSpace(item.ParentExternalidentifier)
                ? null
                : _clashRepository.FindByExternal(item.ParentExternalidentifier)?.FirstOrDefault();

            return Ok(_mapper.Map<ClashModel>(Tuple.Create(item, parentclash)));
        }

        /// <summary>
        /// Creates a set of Clash
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Clash")]
        public IHttpActionResult Post([FromBody] IEnumerable<CreateClash> clashes)
        {
            if (clashes == null || !ModelState.IsValid)
            {
                return InternalServerError();
            }
            var newClashes = _mapper.Map<List<Clash>>(clashes);
            var allClashes = _clashRepository.GetAll()?.ToList();

            try
            {
                Validation(newClashes, allClashes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            foreach (var b in newClashes)
            {
                b.Uid = Guid.NewGuid();

                var validationResult = ValidateClashDifferencesForSave(b, allClashes);
                if (!validationResult.Successful)
                {
                    return this.Error().BadRequest(ApiError.BadRequest(validationResult.Message));
                }
            }
            _clashRepository.Add(newClashes);

            return Ok();
        }

        /// <summary>
        /// Update Clash
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Clash")]
        public IHttpActionResult Put(Guid id, [FromBody] UpdateClashModel command, bool applyGlobally = false)
        {
            if (!ModelState.IsValid || id == default || command == null)
            {
                return this.Error().InvalidParameters();
            }

            var clash = _clashRepository.Get(id);

            if (clash == null)
            {
                return NotFound();
            }

            var updateResult = ValidateAndSaveClash(clash, command, applyGlobally);

            return updateResult.Success
                ? Ok(clash)
                : this.Error().BadRequest(updateResult.Message);
        }

        /// <summary>
        /// Deletes a single Clash by id
        /// </summary>
        /// <param name = "id" ></param >
        [Route("{id}")]
        [AuthorizeRequest("Clash")]
        public IHttpActionResult Delete(Guid id)
        {
            if (id == default)
            {
                return this.Error().InvalidParameters();
            }

            var clashDeleteResult = DeleteClashById(id);

            return clashDeleteResult.Success
                ? Ok()
                : this.Error().BadRequest(clashDeleteResult.Message);
        }

        /// <summary>
        /// Deletes a single Clash by external reference
        /// </summary>
        /// <param name = "externalReference"></param >
        [Route("External/{externalReference}")]
        [AuthorizeRequest("Clash")]
        public IHttpActionResult Delete(string externalReference)
        {
            if (string.IsNullOrWhiteSpace(externalReference))
            {
                return this.Error().InvalidParameters();
            }

            var clashDeleteResult = DeleteClashByExternalId(externalReference);

            return clashDeleteResult.Success
                ? Ok()
                : this.Error().BadRequest(clashDeleteResult.Message);
        }

        /// <summary>
        /// Delete all Clashes
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("Clash")]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Clash>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _clashRepository.TruncateAsync();

            return Ok();
        }

        /// <summary>
        /// Validate a set of Clash
        /// </summary>
        [Route("ValidateAll")]
        [AuthorizeRequest("Clash")]
        [HttpGet]
        public IHttpActionResult Validate()
        {
            // validate exisiting clash from DB
            var allclash = _clashRepository.GetAll()?.ToList();
            ClashRequiredValidation(allclash);
            ClashHelper.ValidateClashes(allclash);
            return Ok();
        }

        private void Validation(List<Clash> newClashes, List<Clash> clashes)
        {
            ClashRequiredValidation(newClashes);
            ClashHelper.ValidateClashes(newClashes, clashes);
        }

        private static void ClashRequiredValidation(List<Clash> clashes)
        {
            clashes.ForEach(c => Clash.Validation(c.Externalref, c.Description, c.DefaultPeakExposureCount, c.DefaultOffPeakExposureCount));
        }

        [Route("SearchAll")]
        [AuthorizeRequest("Clash")]
        [ResponseType(typeof(SearchResultModel<ClashNameModel>))]
        public IHttpActionResult Get([FromUri] ClashSearchQueryModel queryModel)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("One or more of the required query parameters are missing.");
            }
            if (queryModel == null)
            {
                queryModel = new ClashSearchQueryModel();
            }
            var clashes = _clashRepository.Search(queryModel);

            var searchModel = new SearchResultModel<ClashNameModel>
            {
                Items = clashes.Items.ToList(),
                TotalCount = clashes.TotalCount
            };
            return Ok(searchModel);
        }
    }
}
