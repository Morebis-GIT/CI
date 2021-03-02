using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace xggameplan.Controllers.CustomHttpResults
{
    /// <summary>
    /// Returns 422 (Unprocessable Entity) response
    /// <seealso cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/422"/>"
    /// </summary>
    public class UnprocessableEntityResult : IHttpActionResult
    {
        private readonly HttpRequestMessage _request;

        public UnprocessableEntityResult(HttpRequestMessage request)
        {
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = (HttpStatusCode)422,
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }

    }
}
