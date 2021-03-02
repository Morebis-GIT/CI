using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;

namespace xggameplan.Areas.System.Auth
{
    public interface IAuthenticationManager
    {
        bool TrySignIn(string email, string password, out AccessToken token);

        User GetAuthenticatedUser(string token);

        void SignOut(string token);
    }
}
