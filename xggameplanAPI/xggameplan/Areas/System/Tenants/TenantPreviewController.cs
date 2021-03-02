using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.Areas.System.Tenants.Models;
using xggameplan.Controllers.CustomHttpResults;
using xggameplan.Filters;

namespace xggameplan.Areas.System.Tenants
{
    /// <summary>
    /// Operations for Tenant preview.
    /// </summary>
    [RoutePrefix("tenants/{id}/preview")]
    public class TenantPreviewController : ApiController
    {
        private readonly ITenantsRepository _tenantsRepository;
        /// <summary>
        /// Constructor.
        /// </summary>
        public TenantPreviewController(ITenantsRepository tenantsRepository)
        {
            _tenantsRepository = tenantsRepository;
        }

        /// <summary>
        /// Downloads Tenants Preview
        /// </summary>
        /// <param name="id"> Tenant id</param>
        [AuthorizeRequest("ViewTenants")]
        [ResponseType(typeof(TenantModel))]
        [Route("")]
        public IHttpActionResult Get(int id)
        {
            Stream stream;
            PreviewFile previewFile = _tenantsRepository.GetPreviewFile(entityId: id, out stream);
            
            if (stream == null || previewFile == null)
            {
                return NotFound();
            }
            return new ContentStreamActionResult(Request,
                                                  previewFile.FileName,
                                                  previewFile.ContentType,
                                                  stream);
        }

        /// <summary>
        /// Upload Preview.
        /// </summary>
        /// <param name="id">Tenant id</param>
        [AuthorizeRequest("ModifyTenants")]
        [Route("")]
        public IHttpActionResult Post([FromUri] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            var multipartStreamProvider = Request.Content.ReadAsMultipartAsync().Result;
            var sContent = multipartStreamProvider.Contents.SingleOrDefault();
            if (sContent == null)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var user = _tenantsRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            var headers = sContent.Headers;
            var previewFile = PreviewFile.Create(
                                    filename: headers.ContentDisposition?.FileName,
                                    contentType: headers.ContentType.MediaType,
                                    contentLength: headers.ContentLength,
                                    location: "tenants");

            using (var contentStream = sContent.ReadAsStreamAsync().Result)
            {
                _tenantsRepository.SetPreviewFile(entityId: id, previewFile, contentStream);
            }
            _tenantsRepository.SaveChanges();
            return Ok();
        }


        /// <summary>
        /// Deletes preview.
        /// </summary>
        /// <param name="id">Tenant id</param>
        [AuthorizeRequest("ModifyTenants")]
        [Route("")]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            var tenant = _tenantsRepository.GetById(id);
            var preview = tenant?.Preview;
            if (preview == null)
            {
                return NotFound();
            }
            _tenantsRepository.DeletePreviewFile(entitiId: id);
            _tenantsRepository.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
