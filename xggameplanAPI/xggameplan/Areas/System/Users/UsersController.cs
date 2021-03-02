using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using NodaTime;
using xggameplan.Areas.System.Users.Models;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using SystemDomain = ImagineCommunications.GamePlan.Domain.Shared.System;

namespace xggameplan.Areas.System.Users
{
    /// <summary>
    /// Provides methods for managing users registered in the system.
    /// </summary>
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all registered users. Valid access token needs to be provided.
        /// </summary>
        /// <returns>List of registered users</returns>
        [AuthorizeRequest("ViewUsers")]
        [Route("")]
        public List<UserModel> Get() => _mapper.Map<List<UserModel>>(_usersRepository.GetAll());

        /// <summary>
        /// Returns user by id. User must have ViewUsers permission to call this API.
        /// </summary>
        /// <param name="id">User userId</param>
        /// <returns>User with the specified id</returns>
        [Route("{id}")]
        [AuthorizeRequest("ViewUsers")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Get(int id)
        {
            var user = _usersRepository.GetById(id);
            if (user == null)
            {
                return this.Error().ResourceNotFound("User was not found");
            }

            var userModel = _mapper.Map<UserModel>(user);
            return Ok(userModel);
        }

        /// <summary>
        /// Creates User. User must have ModifyUsers permission to call this API.
        /// </summary>
        /// <param name="command">New tenant values</param>
        [Route("")]
        [AuthorizeRequest("ModifyUsers")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Post(
            [FromBody] CreateUserModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("User body parameters missing");
            }
            if (String.IsNullOrWhiteSpace(command.Region))
            {
                return this.Error().InvalidParameters("Region cannot be empty");
            }
            if (String.IsNullOrWhiteSpace(command.ThemeName))
            {
                return this.Error().InvalidParameters("Theme Name cannot be empty");
            }

            var user = SystemDomain.Users.User.Create(0,
                                        command.Name,
                                        command.Surname,
                                        command.Email,
                                        null,         // Set by UserPasswordController
                                        command.ThemeName.ToLower(),
                                        command.Location,
                                        command.Role,
                                        command.TenantId,
                                        DateTimeZoneProviders.Tzdb[command.Region].ToString());
            if (_usersRepository.GetByEmail(command.Email)?.Id != null)
            {
                return this.Error().InvalidParameters("Email address is already in use");
            }
            _usersRepository.Insert(user);
            _usersRepository.SaveChanges();
            var userModel = _mapper.Map<UserModel>(user);
            return Ok(userModel);
        }

        /// <summary>
        /// Updates user. User must have ModifyUsers permission to call this API.
        /// </summary>
        /// <param name="id">User userId</param>
        /// <param name="command">Updated user values</param>
        [AuthorizeRequest("ModifyUsers")]
        [Route("{id}")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Put(
            [FromUri] int id,
            [FromBody] CreateUserModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("User body parameters missing");
            }
            if (String.IsNullOrWhiteSpace(command.Region))
            {
                return this.Error().InvalidParameters("Region cannot be empty");
            }
            if (String.IsNullOrWhiteSpace(command.ThemeName))
            {
                return this.Error().InvalidParameters("Theme Name cannot be empty");
            }

            var user = _usersRepository.GetById(id);
            if (user == null)
            {
                return this.Error().ResourceNotFound("User was not found");
            }

            byte[] previewFile = _usersRepository.GetContent(entityId: id);
            if (previewFile != null)
            {
                user.Preview.SetContent(previewFile);
            }

            user.Change(command.Name,
                        command.Surname,
                        command.Email,
                        command.ThemeName.ToLower(),
                        command.Location,
                        command.Role,
                        command.TenantId,
                        DateTimeZoneProviders.Tzdb[command.Region].ToString());

            var userByEmail = _usersRepository.GetByEmail(command.Email);
            if (userByEmail != null && userByEmail.Id != user.Id)
            {
                return this.Error().InvalidParameters("Email address is already in use");
            }
            _usersRepository.Update(user);
            _usersRepository.SaveChanges();

            var userModel = _mapper.Map<UserModel>(user);
            return Ok(userModel);
        }

        /// <summary>
        /// Updates user password. User must have ModifyUsers permission to call this API.
        /// </summary>
        /// <param name="id">User userId</param>
        /// <param name="command">Updated password values</param>
        [Route("{id}/password")]
        [AuthorizeRequest("ModifyUsers")]
        [ResponseType(typeof(string))]
        [HttpPut]
        public IHttpActionResult UpdatePassword(
            [FromUri] int id,
            [FromBody] UpdatePasswordModel command)
        {
            var user = _usersRepository.GetById(command.Id);
            if (user == null)
            {
                return this.Error().ResourceNotFound("User was not found");
            }

            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Password parameters missing");
            }

            if (String.IsNullOrWhiteSpace(command.CurrentPassword))
            {
                return this.Error().InvalidParameters("Current Password cannot be empty");
            }

            if (user.Password?.HashedValue != null)
            {
                if (!user.IsPasswordValid(command.CurrentPassword))
                {
                    return this.Error().InvalidParameters("Current Password is invalid");
                }
            }

            if (String.IsNullOrWhiteSpace(command.NewPassword))
            {
                return this.Error().InvalidParameters("New Password cannot be empty");
            }

            // Change password
            user.ChangePassword(command.NewPassword, GetBannedPasswordLiteralList(user), GetBannedPasswordPatternList(user));
            _usersRepository.Update(user);
            _usersRepository.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Returns list of banned password patterns for user
        ///
        /// TODO: Enhance this later, read from repository
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Banned passwords (Patterns)</returns>
        private List<string> GetBannedPasswordPatternList(User user) => new List<string>();

        /// <summary>
        /// Returns list of banned password literals for user
        ///
        /// TODO: Enhance this later, add common ones such as "Password[blah]", "abc123" etc, read from repository
        /// </summary>
        /// <param name="user">List</param>
        /// <returns>Banned passwords (Literals)</returns>
        private List<string> GetBannedPasswordLiteralList(User user)
        {
            var list = new List<string>() { user.Name, user.Surname, user.Email };
            list.RemoveAll(item => String.IsNullOrWhiteSpace(item));
            return list;
        }
    }
}
