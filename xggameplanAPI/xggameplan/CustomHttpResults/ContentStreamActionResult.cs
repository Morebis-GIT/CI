using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace xggameplan.Controllers.CustomHttpResults
{
    public class ContentStreamActionResult : IHttpActionResult
    {
        private readonly HttpResponseMessage _response;

        public ContentStreamActionResult(HttpRequestMessage request, string filename, string contentType, Stream stream)
        {
            _response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new StreamContent(stream)
            };

            var headers = _response.Content.Headers;
            headers.ContentType = new MediaTypeHeaderValue(contentType);
            headers.Add("IsCompressedContent", "true");
            headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = filename,
            };
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
            return Task.FromResult(_response);
        }

        /*  public void Dispose()
          {
              _response.Content.Dispose();  ?
          }*/
    }
}
