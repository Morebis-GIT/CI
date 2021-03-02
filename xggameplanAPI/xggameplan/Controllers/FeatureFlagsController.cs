using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.Filters;
using xggameplan.model.External;

namespace xggameplan.Controllers
{
    [RoutePrefix("FeatureFlags")]
    public class FeatureFlagsController : ApiController
    {
        private readonly IFeatureManager _featureManager;
        private readonly IMapper _mapper;

        public FeatureFlagsController(IFeatureManager featureManager, IMapper mapper)
        {
            _featureManager = featureManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns current feature flags state
        /// </summary>
        [Route("")]
        [AuthorizeRequest("FeatureFlags")]
        [ResponseType(typeof(IEnumerable<FeatureStateModel>))]
        public IHttpActionResult Get(bool refresh = false)
        {
            if (refresh)
            {
                _featureManager.ClearCache();
            }

            var sharedFeatures = _featureManager.Features.Where(f => f.IsShared);
            return Ok(_mapper.Map<List<FeatureStateModel>>(sharedFeatures));
        }
    }
}
