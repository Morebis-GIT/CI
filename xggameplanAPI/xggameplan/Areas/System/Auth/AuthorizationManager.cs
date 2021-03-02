using System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;

namespace xggameplan.Areas.System.Auth
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private readonly IAuthenticationManager _authenticationManager;

        public AuthorizationManager(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        /// <summary>
        /// Checks if access token exists and doesn't expired. Action is not checked for now.
        /// </summary>
        public bool IsAuthorized(string token, string action)
        {
            _ = token ?? throw new ArgumentException(nameof(token));
            _ = action ?? throw new ArgumentException(nameof(action));

            var user = _authenticationManager.GetAuthenticatedUser(token);

            return IsUserAuthorizedForAction(user,action);
        }

        public bool IsAuthorized(User user, string action)
        {
            _ = user ?? throw new ArgumentException(nameof(user));
            _ = action ?? throw new ArgumentException(nameof(action));

            return IsUserAuthorizedForAction(user, action);
        }

        private bool IsUserAuthorizedForAction(User user, string action)
        {
            // TODO: should be reviewed, now it's original nextgen logic
            return user != null;
        }
    }
}
