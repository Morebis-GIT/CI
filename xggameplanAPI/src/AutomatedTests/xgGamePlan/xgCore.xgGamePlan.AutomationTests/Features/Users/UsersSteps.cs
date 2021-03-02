using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Tenants;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Contexts;
using xgCore.xgGamePlan.AutomationTests.Coordinators;
using xgCore.xgGamePlan.AutomationTests.Infrastructure;

namespace xgCore.xgGamePlan.AutomationTests.Features.Users
{
    [Binding]
    public class UsersSteps
    {
        private readonly IUsersApi _usersApi;
        private readonly UsersCoordinator _usersCoordinator;
        private readonly AuthContext _authContext;
        private readonly UsersContext _usersContext;

        public UsersSteps(
            IUsersApi usersApi,
            UsersCoordinator usersCoordinator,
            AuthContext authContext,
            UsersContext usersContext)
        {
            _usersApi = usersApi;
            _usersCoordinator = usersCoordinator;
            _authContext = authContext;
            _usersContext = usersContext;
        }

        [Given(@"I have valid user credentials")]
        public async Task GivenIHaveValidUserCredentialsAsync()
        {
            _authContext.UserCredentials = await _usersCoordinator.CreateUserWithCredentials().ConfigureAwait(false);
        }

        [Given(@"I have invalid user credentials")]
        public void GivenIHaveInvalidUserCredentials()
        {
            _authContext.UserCredentials = new UserCredentials
            {
                Id = 0,
                Email = $"{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)}@domain.com",
                Password = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
        }

        [Given(@"I know how many Users there are")]
        public async Task GivenHowManyUserThereAreAsync()
        {
            _usersContext.InitialUsers = await _usersApi.GetAll().ConfigureAwait(false);
        }

        [Given(@"I have added a User")]
        public async Task GivenIHaveAddedUserAsync()
        {
            _usersContext.GivenUser = await _usersCoordinator.CreateUserAsync().ConfigureAwait(false);
        }

        [When(@"I add (\d+) Users")]
        public async Task WhenIAddUsersAsync(int count)
        {
            await Task.WhenAll(Enumerable.Repeat(0, count).Select(x => _usersCoordinator.CreateUserAsync()))
                .ConfigureAwait(false);
        }

        [When(@"I request my User by ID")]
        public async Task WhenIRequestMyUserByIdAsync()
        {
            Assert.NotNull(_usersContext.GivenUser, "There is no given user.");
            _usersContext.ReturnedUser =
                await _usersApi.GetById(_usersContext.GivenUser.Id).ConfigureAwait(false);
        }

        [When(@"I update User by ID")]
        public async Task WhenIUpdateUserByIdAsync()
        {
            Assert.NotNull(_usersContext.GivenUser, "There is no given user.");
            var userUpdate = _usersCoordinator.BuildUserModel();
            _usersContext.ReturnedUser = await _usersApi.Update(_usersContext.GivenUser.Id, userUpdate)
                .ConfigureAwait(false);
        }

        [Then(@"(\d+) additional Users are returned")]
        public async Task ThenAdditionalUsersAreReturnedAsync(int count)
        {
            var users = await _usersApi.GetAll().ConfigureAwait(false);
            Assert.AreEqual(count, users.Count - _usersContext.InitialUsers.Count);
        }

        [Then(@"requested User with ID is returned")]
        public void ThenRequestedUserWithIdIsReturnedAsync()
        {
            Assert.AreEqual(_usersContext.GivenUser?.Id, _usersContext.ReturnedUser?.Id);
        }

        [Then(@"updated User is returned")]
        public async void ThenUpdatedUserIsReturnedAsync()
        {
            Assert.NotNull(_usersContext.ReturnedUser, "There is no updated user.");
            var user = await _usersApi.GetById(_usersContext.ReturnedUser.Id).ConfigureAwait(false);
            Assert.AreEqual(_usersContext.ReturnedUser, user);
        }
    }
}
