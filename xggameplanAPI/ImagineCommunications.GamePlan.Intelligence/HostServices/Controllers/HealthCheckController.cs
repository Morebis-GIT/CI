using System.Web.Http;

namespace ImagineCommunications.GamePlan.Intelligence.HostServices.Controllers
{
    [RoutePrefix("api"), Route("healthcheck")]
    public class HealthCheckController : ApiController
    {
        [HttpGet]
        public IHttpActionResult HealthCheck() => Ok();
    }
}
