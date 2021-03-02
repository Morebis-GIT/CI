using System;
using System.Globalization;
using xgCore.xgGamePlan.ApiEndPoints.Models.AccessTokens;
using xgCore.xgGamePlan.ApiEndPoints.Models.Users;
using xgCore.xgGamePlan.AutomationTests.Infrastructure;

namespace xgCore.xgGamePlan.AutomationTests.Extensions
{
    public static class UserExtensions
    {
        public static UserCredentials ToUserCredentials(this UserModel userModel, string password = null)
        {
            if (userModel == null)
            {
                return null;
            }

            return new UserCredentials
            {
                Id = userModel.Id,
                Email = userModel.Email,
                Password = password ?? Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)
            };
        }

        public static AccessTokenCommand ToAccessTokenCommand(this UserCredentials userCredentials)
        {
            if (userCredentials == null)
            {
                return null;
            }

            return new AccessTokenCommand
            {
                Email = userCredentials.Email,
                Password = userCredentials.Password
            };
        }
    }
}
