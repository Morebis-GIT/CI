using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using xggameplan.Controllers.CustomHttpResults;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Areas.System.Users
{
    /// <summary>
    /// Operations for Users preview.
    /// </summary>
    [RoutePrefix("users/{id}/preview")]
    public class UsersPreviewController : ApiController
    {
        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        public UsersPreviewController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Downloads User Preview.
        /// </summary>
        /// <param name="id"> User id</param>
        [HttpGet]
        [Route("")]
        [AuthorizeRequest("ViewUsers")]
        [ResponseType(typeof(UserModel))]

        public IHttpActionResult Get(int id)
        {
            Stream stream;
            PreviewFile previewFile = _usersRepository.GetPreviewFile(entityId: id, out stream);
            if (stream == null || previewFile == null)
            {
                return NotFound();
            }
            Request.RegisterForDispose(stream);
            return new ContentStreamActionResult(Request,
                                              previewFile.FileName,
                                              previewFile.ContentType,
                                              stream);
        }

        /// <summary>
        /// Uploads User Preview.
        /// </summary>
        /// <param name="id"> User id</param>
        [Route("")]
        [AuthorizeRequest("ModifyUsers")]
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

            var user = _usersRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            var headers = sContent.Headers;
            var previewFile = PreviewFile.Create(
                                    filename: headers.ContentDisposition?.FileName,
                                    contentType: headers.ContentType.MediaType,
                                    contentLength: headers.ContentLength,
                                    location: "users");

            using (var contentStream = sContent.ReadAsStreamAsync().Result)
            {
                _usersRepository.SetPreviewFile(entityId: id, previewFile, contentStream);
            }
            _usersRepository.SaveChanges();
            
            return Ok();
        }

        /// <summary>
        /// Deletes preview.
        /// </summary>
        /// <param name="id">User id</param>
        [Route("")]
        [AuthorizeRequest("ModifyUsers")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            var user = _usersRepository.GetById(id);
            var preview = user?.Preview;
            if (preview == null)
            {
                return NotFound();
            }
            _usersRepository.DeletePreviewFile(entitiId: id);
            _usersRepository.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
