using System.Web.Http;
using System.Web.Http.Description;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using xggameplan.Areas.System.Users.Models;
using xggameplan.Filters;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using System.Web;
using AutoMapper;
using xggameplan.Extensions;

namespace xggameplan.Areas.System.Users
{
    [RoutePrefix("me")]
    public class CurrentUserController : ApiController
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CurrentUserController(IMapper mapper, IUsersRepository usersRepository, ITenantsRepository tenantsRepository)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns user by id. User must have ViewUsers permission to call this API.
        /// </summary>
        /// <param name="id">User userId</param>
        /// <returns>User with the specified id</returns>
        [Route("")]
        [AuthorizeRequest("ViewUsers")]
        [ResponseType(typeof(CurrentUserModel))]
        public IHttpActionResult Get()
        {
            var user = HttpContext.Current.GetCurrentUser();//_requestExecutionContext.User;
            var userModel = _mapper.Map<CurrentUserModel>(user);
            userModel.Tenant = _mapper.Map<CurrentUserTenantModel>(_tenantsRepository.GetById(user.TenantId));
            return Ok(userModel);
        }
    }
}
