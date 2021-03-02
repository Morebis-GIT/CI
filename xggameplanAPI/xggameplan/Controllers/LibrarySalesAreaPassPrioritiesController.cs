using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.Controllers.CustomHttpResults;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.Controllers
{
    /// <summary>
    /// Library SalesAreaPassPriorities Controller
    /// </summary>
    [RoutePrefix("Library")]
    public class LibrarySalesAreaPassPrioritiesController : ApiController
    {
        private const string AuthorizeRequestActionName = "LibrarySalesAreaPassPriorities";
        private const string RoutePrefix = "SalesAreaPassPriorities";
        private readonly IMapper _mapper;
        private readonly ILibrarySalesAreaPassPrioritiesRepository _librarySalesAreaPassPrioritiesRepository;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly IModelDataValidator<CreateLibrarySalesAreaPassPriorityModel> _validatorForCreate;
        private readonly IModelDataValidator<UpdateLibrarySalesAreaPassPriorityModel> _validatorForUpdate;

        /// <summary>
        /// Create an instance of  <see cref="LibrarySalesAreaPassPrioritiesController"/>
        /// </summary>
        /// <param name="librarySalesAreaPassPrioritiesRepository">The repository to use for LibrarySalesAreaPassPriority</param>
        /// <param name="tenantSettingsRepository">The repository to use for TenantSettings</param>
        /// <param name="mapper">The mapper to use for Model Mappings</param>
        /// <param name="validatorForCreate">The Validator For Create</param>
        /// <param name="validatorForUpdate">The Validator For Update</param>
        public LibrarySalesAreaPassPrioritiesController(ILibrarySalesAreaPassPrioritiesRepository librarySalesAreaPassPrioritiesRepository,
                                                        ITenantSettingsRepository tenantSettingsRepository,
                                                        IMapper mapper,
                                                        IModelDataValidator<CreateLibrarySalesAreaPassPriorityModel> validatorForCreate,
                                                        IModelDataValidator<UpdateLibrarySalesAreaPassPriorityModel> validatorForUpdate)
        {
            _mapper = mapper;
            _librarySalesAreaPassPrioritiesRepository = librarySalesAreaPassPrioritiesRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _validatorForCreate = validatorForCreate;
            _validatorForUpdate = validatorForUpdate;
        }

        /// <summary>
        /// Gets all Sales Area Pass Priorities from the library
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> if successfull.</returns>
        /// <response code="200">Returns a collection of Sales Area Pass Priority library items with a 200 OK Reponse code</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [HttpGet, Route(RoutePrefix)]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        [ResponseType(typeof(IEnumerable<LibrarySalesAreaPassPriorityModel>))]
        public async Task<IHttpActionResult> GetAllAsync()
        {
            var entities = await _librarySalesAreaPassPrioritiesRepository.GetAllAsync();
            var result = _mapper.Map<List<LibrarySalesAreaPassPriorityModel>>(entities);

            var defaultSalesAreaPassPriorityId = GetDefaultSalesAreaPassPriorityId();
            if (defaultSalesAreaPassPriorityId != Guid.Empty)
            {
                var defaultFoundAt = result.FindIndex(a => a.Uid.Equals(defaultSalesAreaPassPriorityId));
                if (defaultFoundAt >= 0) { result[defaultFoundAt].IsDefault = true; }
            }

            return Ok(result.OrderByDescending(a => a.IsDefault).ThenBy(a => a.Name).AsEnumerable());
        }

        /// <summary>
        /// Get a Sales Area Pass Priority library item for the supplied id
        /// </summary>
        /// <param name="id" type="Guid">The Unique Identifier of the Library SalesAreaPassPriority </param>
        /// <returns><see cref="LibrarySalesAreaPassPriorityModel"/> if successfull</returns>
        /// <response code="200">Returns a LibrarySalesAreaPassPriority with 200 OK response, when a matching record is found for the supplied id</response>
        /// <response code="404">Returns 404 NOT Found response, when no matching record is found for the supplied id</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [HttpGet, Route(RoutePrefix + "/{id:Guid}", Name = RoutePrefix)]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        [ResponseType(typeof(LibrarySalesAreaPassPriorityModel))]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            LibrarySalesAreaPassPriority entity;
            if (id == Guid.Empty || (entity = await _librarySalesAreaPassPrioritiesRepository.GetAsync(id)) == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<LibrarySalesAreaPassPriorityModel>(entity);
            result.IsDefault = IsItDefault(result.Uid);

            return Ok<LibrarySalesAreaPassPriorityModel>(result);
        }

        /// <summary>
        /// Get the Default Sales Area Pass Priority library item
        /// </summary>
        /// <returns><see cref="LibrarySalesAreaPassPriorityModel"/> if a Default Sales Area Pass Priority library item is found successfully</returns>
        /// <response code="200">Returns a LibrarySalesAreaPassPriority with 200 OK response, when a Default Sales Area Pass Priority library item is found</response>
        /// <response code="404">Returns 404 NOT Found response, when no Default Sales Area Pass Priority library item is found</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [HttpGet, Route(RoutePrefix + "/Default")]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        [ResponseType(typeof(LibrarySalesAreaPassPriorityModel))]
        public async Task<IHttpActionResult> GetDefaultAsync()
        {
            var defaultSalesAreaPassPriorityId = GetDefaultSalesAreaPassPriorityId();
            if (defaultSalesAreaPassPriorityId == Guid.Empty)
            {
                return NotFound();
            }

            return await GetAsync(defaultSalesAreaPassPriorityId);
        }

        /// <summary>
        /// Creates a Sales Area Pass Priority library item based on the supplied model
        /// </summary>
        /// <param name="model" type="CreateLibrarySalesAreaPassPriorityModel">The CreateLibrarySalesAreaPassPriorityModel</param>
        /// <returns>The created <see cref="LibrarySalesAreaPassPriorityModel"/> if successful</returns>
        /// <response code="201">Returns the created Library SalesAreaPassPriority with 201 Created response</response>
        /// <response code="400">Returns 400 Bad Request, when the supplied model is invalid/fails validation</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        /// <remarks>
        /// Sample request:
        ///
        /// {
        ///   "name": "priority 101",
        ///   "startTime": "06:00",
        ///   "endTime": "07:00",
        ///   "daysOfWeek": "1111101",
        ///   "salesAreaPriorities": [
        ///     {
        ///       "salesArea": "NWS93",
        ///       "priority": "Exclude"
        ///     },
        ///     {
        ///       "salesArea": "GTV93",
        ///       "priority": "Priority1"
        ///     }
        ///   ]
        /// }
        ///
        /// </remarks>
        [HttpPost, Route(RoutePrefix)]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        [ResponseType(typeof(LibrarySalesAreaPassPriorityModel))]
        public async Task<IHttpActionResult> PostAsync([FromBody] CreateLibrarySalesAreaPassPriorityModel model)
        {
            if (!_validatorForCreate.IsValid(model))
            {
                return _validatorForCreate.BadRequest();
            }

            var entity = _mapper.Map<LibrarySalesAreaPassPriority>(model);
            entity.Uid = Guid.NewGuid();

            await _librarySalesAreaPassPrioritiesRepository.AddAsync(entity);
            await _librarySalesAreaPassPrioritiesRepository.SaveChanges();

            var result = _mapper.Map<LibrarySalesAreaPassPriorityModel>(entity);
            return CreatedAtRoute<LibrarySalesAreaPassPriorityModel>(RoutePrefix, new { id = result.Uid }, result);
        }

        /// <summary>
        /// Update a Sales Area Pass Priority library item based on the supplied model
        /// </summary>
        /// <param name="model" type="UpdateLibrarySalesAreaPassPriorityModel">The UpdateLibrarySalesAreaPassPriorityModel</param>
        /// <returns>The updated <see cref="LibrarySalesAreaPassPriorityModel"/> if successfull</returns>
        /// <response code="200">Returns the updted LibrarySalesAreaPassPriority with 200 OK Response, when the update is successfull</response>
        /// <response code="404">Returns 404 NOT Found response, when no matching record is found to perform the update</response>
        /// <response code="400">Returns 400 Bad Request, when the supplied model is invalid/fails validation</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        /// <remarks>
        /// Sample request:
        ///
        /// {
        ///   "name": "priority 101 v1",
        ///   "startTime": "06:30",
        ///   "endTime": "07:00",
        ///   "daysOfWeek": "1001101",
        ///   "salesAreaPriorities": [
        ///     {
        ///       "salesArea": "NWS93",
        ///       "priority": "Exclude"
        ///     },
        ///     {
        ///       "salesArea": "GTV93",
        ///       "priority": "Priority2"
        ///     }
        ///   ]
        /// }
        ///
        /// </remarks>
        [HttpPut, Route(RoutePrefix)]
        [AuthorizeRequest(AuthorizeRequestActionName)]
        [ResponseType(typeof(LibrarySalesAreaPassPriorityModel))]
        public async Task<IHttpActionResult> PutAsync(Guid id, [FromBody] UpdateLibrarySalesAreaPassPriorityModel model)
        {
            model.Uid = id;
            if (!_validatorForUpdate.IsValid(model))
            {
                return _validatorForUpdate.BadRequest();
            }

            LibrarySalesAreaPassPriority entity;
            if (id == Guid.Empty || (entity = await _librarySalesAreaPassPrioritiesRepository.GetAsync(id)) == null)
            {
                return NotFound();
            }

            //NextGen.Core.Services.IMapper implementation currently doesn't support source and destination mapping hence this code
            _mapper.Map(model, entity);
            await _librarySalesAreaPassPrioritiesRepository.UpdateAsync(entity);
            await _librarySalesAreaPassPrioritiesRepository.SaveChanges();

            var result = _mapper.Map<LibrarySalesAreaPassPriorityModel>(entity);
            result.IsDefault = IsItDefault(result.Uid);
            return Ok<LibrarySalesAreaPassPriorityModel>(result);
        }

        /// <summary>
        /// Delete Sales Area Pass Priority from the library
        /// </summary>
        /// <param name="id">Sales Area Pass Priority Id</param>
        [HttpDelete, Route(RoutePrefix)]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Id");
            }

            LibrarySalesAreaPassPriority libSalesAreaPassPriority = await _librarySalesAreaPassPrioritiesRepository.GetAsync(id);
            if (libSalesAreaPassPriority == null)
            {
                return NotFound();
            }

            // Check that sales area pass priority being deleted isn't the default
            if (libSalesAreaPassPriority.Uid == GetDefaultSalesAreaPassPriorityId())
            {
                return new UnprocessableEntityResult(Request);
            }

            await _librarySalesAreaPassPrioritiesRepository.Delete(libSalesAreaPassPriority.Uid).ConfigureAwait(false);
            return this.NoContent();
        }

        /// <summary>
        /// Set Sales Area Pass Priority as default
        /// </summary>
        /// <param name="id">Sales Area Pass Priority Id</param>
        [HttpPut, Route(RoutePrefix + "/default")]
        public async Task<IHttpActionResult> SetDefault(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Id");
            }

            LibrarySalesAreaPassPriority libSalesAreaPassPriority = await _librarySalesAreaPassPrioritiesRepository.GetAsync(id);
            if (libSalesAreaPassPriority == null)
            {
                return NotFound();
            }

            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            if (tenantSettings != null)
            {
                tenantSettings.DefaultSalesAreaPassPriorityId = id;
                _tenantSettingsRepository.AddOrUpdate(tenantSettings);
            }
            return Ok();
        }

        /// <summary>
        /// To verify if the supplied uId <see cref="Guid"/>, is the Uid of the default SalesArea Pass Priority Id
        /// as setup in the Tenant Settings.
        /// </summary>
        /// <param name="uId">the uId <see cref="Guid"/></param>
        /// <returns>True or False to indicate whether the supplied uId, is the Uid of
        ///          the default SalesArea Pass Priority Id as setup in the Tenant Settings.
        /// </returns>
        private bool IsItDefault(Guid uId)
        {
            var defaultSalesAreaPassPriorityId = GetDefaultSalesAreaPassPriorityId();
            return uId != Guid.Empty && defaultSalesAreaPassPriorityId != Guid.Empty && uId.Equals(defaultSalesAreaPassPriorityId);
        }

        private Guid GetDefaultSalesAreaPassPriorityId()
        {
            //var tenantSettings = _tenantSettingsRepository.Find(TenantSettings.DefaultId);
            //return (tenantSettings != null) ? tenantSettings.DefaultSalesAreaPassPriorityId : Guid.Empty;
            return _tenantSettingsRepository.GetDefaultSalesAreaPassPriorityId();
        }
    }
}
