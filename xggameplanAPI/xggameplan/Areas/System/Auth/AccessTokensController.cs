using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using xggameplan.Areas.System.Auth.Models;
using xggameplan.Controllers;
using xggameplan.Errors;
using xggameplan.Filters;

namespace xggameplan.Areas.System.Auth
{
    /// <summary>
    /// Provides methods for managing user's access tokens.
    /// </summary>
    [RoutePrefix("accesstokens")]
    public class AccessTokensController : ApiController
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccessTokensController(IAuthenticationManager authenticationManager, IMapper mapper)
        {
            // NOTE: Works OK if just IMapper is passed
            _authenticationManager = authenticationManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Checks the provided credentials and if the credentials are valid, creates access token.
        /// </summary>
        /// <param name="getAccessTokenModel">User's credentials</param>
        /// <returns>New access token</returns>
        [Route("")]
        [ResponseType(typeof(AccessTokenModel))]
        public IHttpActionResult Post([FromBody] GetAccessTokenModel getAccessTokenModel)
        {
            if (getAccessTokenModel == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("User's email and password need to be specified");
            }

            if (_authenticationManager.TrySignIn(getAccessTokenModel.Email, getAccessTokenModel.Password, out AccessToken accessToken))
            {
                return Ok(_mapper.Map<AccessTokenModel>(accessToken));
            }

            return Unauthorized();
        }

        /// <summary>
        /// Invalidates the access token, so it can no longer be used to authorize requests. User must possess DeleteAccessToken permission to perform this action.
        /// </summary>
        /// <param name="token">Token</param>
        [Route("{token}")]
        [AuthorizeRequest("DeleteAccessToken")]
        public IHttpActionResult Delete(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return this.Error().InvalidParameters("Token needs to be specified");
            }

            _authenticationManager.SignOut(token);

            return this.NoContent();
        }
    }
}
