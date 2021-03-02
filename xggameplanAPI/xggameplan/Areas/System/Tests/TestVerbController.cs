using System.Web.Http;

namespace xggameplan.Areas.System.Tests
{
    /// <summary>
    /// API for testing HTTP verbs
    /// </summary>
    [RoutePrefix("api")]
    [Route("TestVerb")]
    public class TestVerbController
        : ApiController
    {

        public TestVerbController()
        {
          
        }

        /// <summary>
        /// Test the GET verb
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetVerbTest() => Ok();

        /// <summary>
        /// Test the PUT verb
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult PutVerbTest([FromBody] string data) => Ok();

        /// <summary>
        /// Test the POST verb
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult PostVerbTest([FromBody] string data) => Ok();

        /// <summary>
        /// Test the DELETE verb
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult DeleteVerbTest() => Ok();

        /// <summary>
        /// Test the PATCH verb
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public IHttpActionResult PatchVerbTest() => Ok();
    }
}
