using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace xggameplan.Controllers
{
    /// <summary>
    /// Api controller extensions
    /// </summary>
    public static class ApiControllerExtensions
    {
        /// <summary>
        /// Returns 200 OK response with Location header set.
        /// </summary>
        public static IHttpActionResult Ok<TModel>(
            this ApiController controller,
            string location,
            TModel model)
        {
            var response = controller.Request.CreateResponse(
                HttpStatusCode.OK,
                model);

            response.Headers.Add("location", location);

            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// Returns 204 (No Content) response
        /// </summary>
        public static IHttpActionResult NoContent(
            this ApiController controller)
        {
            return new StatusCodeResult(
                HttpStatusCode.NoContent,
                controller);
        }

        public static IHttpActionResult PreconditionFailed<TModel>(
            this ApiController controller,
            TModel model)
        {
            var response = controller.Request.CreateResponse(
                HttpStatusCode.PreconditionFailed,
                model);

            return new ResponseMessageResult(response);
        }
    }
}
