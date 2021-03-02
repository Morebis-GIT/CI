using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using ImagineCommunications.GamePlan.Domain.BRS;
using xggameplan.Filters;

namespace xggameplan.Controllers
{
    [RoutePrefix("KPIPriorities")]
    public class KPIPrioritiesController : ApiController
    {
        private readonly IKPIPriorityRepository _repository;

        public KPIPrioritiesController(IKPIPriorityRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all KPI priorities
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("KPIPriorities")]
        [ResponseType(typeof(IEnumerable<KPIPriority>))]
        public IHttpActionResult GetKPIPriorities()
        {
            return Ok(_repository.GetAll());
        }
    }
}
