using System.Web.Http;

namespace xggameplan.Areas.System.Tests
{
    [RoutePrefix("api"), Route("healthcheck")]
    public class HealthCheckController : ApiController
    {
        [HttpGet]
        public IHttpActionResult HealthCheck() => Ok();
    }
}
