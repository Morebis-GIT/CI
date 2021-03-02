using System.Web.Http;
using System.Web.Http.Description;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Areas.System
{
    [RoutePrefix("api")]
    public class SystemController
        : ApiController
    {
        /// <summary>
        /// Returns API version. Does not require authentication.
        /// </summary>
        /// <returns></returns>
        [Route("Version")]
        [ResponseType(typeof(APIVersionModel))]
        public APIVersionModel Get() =>
            VersionService.GetVersion();
    }
}
