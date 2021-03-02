using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.Areas.System.Tenants.Models;
using xggameplan.Errors;
using xggameplan.Filters;

namespace xggameplan.Areas.System.Tenants
{
    /// <summary>
    /// Provides API methods for manipulating with tenants.
    /// </summary>
    [RoutePrefix("tenants")]
    public class TenantController : ApiController
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TenantController(
            ITenantsRepository tenantsRepository,
            IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all tenants. User must have ViewTenants permission to call this API.
        /// </summary>
        /// <returns>All tenants</returns>
        [Route("")]
        [AuthorizeRequest("ViewTenants")]
        public List<TenantModel> Get()
        {
            var tenants = _tenantsRepository
                .GetAll()
                .OrderBy(x => x.Name)
                .ToList();

            return _mapper.Map<List<TenantModel>>(tenants);
        }

        /// <summary>
        /// Returns tenant by id. User must have ViewTenants permission to call this API.
        /// </summary>
        /// <param name="id">Channel channelId</param>
        /// <returns>Channel with the specified id</returns>
        [Route("{id}")]
        [AuthorizeRequest("ViewTenants")]
        [ResponseType(typeof(TenantModel))]
        public IHttpActionResult Get(int id)
        {
            var tenant = _tenantsRepository.GetById(id);
            if (tenant == null)
            {
                return this.Error().ResourceNotFound("Tenant was not found");
            }

            var tenantModel = _mapper.Map<TenantModel>(tenant);

            return Ok(tenantModel);
        }

        /// <summary>
        /// Creates Tenant. User must have ModifyTenants permission to call this API.
        /// </summary>
        /// <param name="command">New tenant values</param>
        [Route("")]
        [AuthorizeRequest("ModifyTenants")]
        [ResponseType(typeof(TenantModel))]
        public IHttpActionResult Post(
            [FromBody] CreateTenantModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Tenant body parameters missing");
            }

            var dbProviderConfig = CreateDBProviderConfiguration(command);

            var tenant = Tenant.Create(0, command.Name, command.DefaultTheme, dbProviderConfig);

            _tenantsRepository.Add(tenant);
            _tenantsRepository.SaveChanges();

            return Ok(_mapper.Map<TenantModel>(tenant));
        }

        private DatabaseProviderConfiguration CreateDBProviderConfiguration(CreateTenantModel command)
            => String.IsNullOrEmpty(command.TenantDbConnectionString)
                    ? DatabaseProviderConfiguration.CreateFromConfiguration(command.TenantDbProvider, command.TenantDbConfiguration)
                    : DatabaseProviderConfiguration.Create(command.TenantDbProvider, command.TenantDbConnectionString);

        /// <summary>
        /// Updates tenant. User must have ModifyTenants permission to call this API.
        /// </summary>
        /// <param name="id">Tenant tenantId</param>
        /// <param name="command">tenant values</param>
        [AuthorizeRequest("ModifyTenants")]
        [Route("{id}")]
        [ResponseType(typeof(TenantModel))]
        public IHttpActionResult Put(
            [FromUri] int id,
            [FromBody] CreateTenantModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Tenant body parameters missing");
            }
            if (command.TenantDbProvider == null || command.TenantDbConfiguration == null)
            {
                return this.Error().InvalidParameters("Tenant Db information missing");
            }

            var tenant = _tenantsRepository.GetById(id);
            tenant.Change(command.Name,
                          command.DefaultTheme,
                          DatabaseProviderConfiguration.Create(command.TenantDbProvider, command.TenantDbConfiguration));
            _tenantsRepository.Update(tenant);
            _tenantsRepository.SaveChanges();
            var tenantModel = _mapper.Map<TenantModel>(tenant);
            return Ok(tenantModel);
        }
    }
}
