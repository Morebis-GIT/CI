using System;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Users;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Extensions;
using xgCore.xgGamePlan.AutomationTests.Infrastructure;

namespace xgCore.xgGamePlan.AutomationTests.Coordinators
{
    [Binding]
    public class UsersCoordinator
    {
        private const string NoPassword = "no_password";

        private readonly IUsersApi _userApi;
        private readonly IFixture _fixture;

        public UsersCoordinator(IUsersApi userApi, IFixture fixture)
        {
            _userApi = userApi;
            _fixture = fixture;
        }

        public CreateUserModel BuildUserModel()
        {
            return _fixture.Build<CreateUserModel>()
                .With(p => p.Region, "Europe/London")
                .With(p => p.Role, "SuperUser")
                .Create();
        }

        public async Task<UserModel> CreateUserAsync()
        {
            return await _userApi.Create(BuildUserModel()).ConfigureAwait(false);
        }

        public async Task<UserCredentials> CreateUserWithCredentials(string password = null)
        {
            var user = await CreateUserAsync().ConfigureAwait(false);
            var updatePasswordModel = new UpdatePasswordModel
            {
                Id = user.Id,
                CurrentPassword = NoPassword,
                NewPassword = password ?? Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
            await _userApi.UpdatePassword(user.Id, updatePasswordModel).ConfigureAwait(false);
            return user.ToUserCredentials(updatePasswordModel.NewPassword);
        }
    }
}
