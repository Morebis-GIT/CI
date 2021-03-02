using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using xggameplan.Filters;
using xggameplan.SystemTests;

namespace xggameplan.Controllers
{
    /// <summary>
    /// System tests API
    /// </summary>
    [RoutePrefix("SystemTests")]
    public class SystemTestsController : ApiController
    {
        private ISystemTestsManager _systemTestsManager;

        public SystemTestsController(ISystemTestsManager systemTestsManager)
        {
            _systemTestsManager = systemTestsManager;
        }

        /// <summary>
        /// Executes system tests for the specified category, returns results
        /// </summary>
        /// <returns></returns>
        [Route("{category}/execute")]
        [AuthorizeRequest("SystemTests")]
        [ResponseType(typeof(List<SystemTestResult>))]
        public IHttpActionResult Execute(SystemTestCategories category = SystemTestCategories.Default)
        {
            var systemTestResults = _systemTestsManager.ExecuteTests(category);
            return Ok(systemTestResults);
        }
    }
}
