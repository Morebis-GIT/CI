using System;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Http;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Microsoft.Extensions.Configuration;
using xggameplan.core.Database;
using xggameplan.core.Export;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.Errors;
using xggameplan.FeatureManagement;
using xggameplan.Filters;
using xggameplan.TestEnv;

namespace xggameplan.Areas.System.Tenants
{
    /// <summary>
    /// Provides API for interacting with tenant database for maintenance functions.
    /// </summary>
    [RoutePrefix("databases/tenant")]
    public class TenantDatabaseController : ApiController
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly ITenantSettingsFeatureManager _featureManager;
        private readonly IFeatureManager _productFeatureManager;
        private readonly IFeatureSettingsProvider _featureSettingsProvider;
        private readonly ITestEnvironment _testEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        public TenantDatabaseController(ITenantsRepository tenantsRepository,
                                        ITenantSettingsRepository tenantSettingsRepository,
                                        ITenantSettingsFeatureManager featureManager,
                                        IFeatureManager productFeatureManager,
                                        IFeatureSettingsProvider featureSettingsProvider,
                                        ITestEnvironment testEnvironment,
                                        IConfiguration configuration,
                                        IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _featureManager = featureManager;
            _productFeatureManager = productFeatureManager;
            _featureSettingsProvider = featureSettingsProvider;
            _testEnvironment = testEnvironment;
            _configuration = configuration;
            _mapper = mapper;
        }

        /// <summary>
        /// For Imagine use only - Activate tenant at runtime without restarting
        /// service. User must have ModifyTenants permission to call this API.
        /// </summary>
        /// <param name="tenantId">Tenant tenantId</param>
        [Route("configuretenant")]
        public IHttpActionResult Put([FromUri] int tenantId)
        {
            // Check tenant exists
            var tenant = _tenantsRepository.GetById(tenantId);
            if (tenant == null)
            {
                return NotFound();
            }

            var featureManager = new FeatureManager(_featureSettingsProvider.GetForTenant(tenantId));
            ApplicationModulesLoader.SetupTenant(tenant, _configuration, _testEnvironment, featureManager);
            return Ok();
        }

        /// <summary>
        /// For Imagine use only - Exports the tenant database, currently just
        /// the data.
        /// </summary>
        /// <param name="tenantId">Tenant tenantId</param>
        [Route("export")]
        public IHttpActionResult PostExport([FromUri] int tenantId)
        {
            var tenant = _tenantsRepository.GetById(tenantId);
            if (tenant == null)
            {
                return NotFound();
            }

            string dataFolder = HostingEnvironment.MapPath($"/Temp/Export/Tenant/SeedData{tenant.Name}");
            new DatabaseJsonExporter(ApplicationModulesLoader.GetTenantDbContext(tenantId))
                .Export(new TenantDatabaseProcessingSettings(dataFolder));
            return Ok();
        }

        /// <summary>
        /// Get a Feature Settings
        /// </summary>
        [Route("FeatureSettings")]
        [AuthorizeRequest("FeatureSettings")]
        public Dictionary<string, object> GetFeatureSettings(string feature) => _featureManager.GetFeatureSettings(feature);

        /// <summary>
        /// Get a Feature Enabled flag
        /// </summary>
        [Route("FeatureEnabled")]
        [AuthorizeRequest("FeatureSettings")]
        public bool GetFeatureEnabled(string feature) => _featureManager.GetFeatureEnabled(feature);

        /// <summary>
        /// Get Start day of week, Peak, Off-peak, Midnight time and System logical date
        /// </summary>
        [Route("TenantTimeSettings")]
        [AuthorizeRequest("TenantTimeSettings")]
        public IHttpActionResult GetTenantTimeSettings()
        {
            try
            {
                var tenantSettings = _tenantSettingsRepository.Get();

                return Ok(tenantSettings.ConvertToTimeTenantSettings(_productFeatureManager));
            }
            catch (Exception ex)
            {
                return this.Error().UnknownError(ex.Message);
            }
        }
    }
}
