using ImagineCommunications.GamePlan.Domain.Shared.System.Users;

namespace xggameplan.Areas.System.Auth
{
    public interface IAuthorizationManager
    {
        bool IsAuthorized(string token, string action);
        bool IsAuthorized(User user, string action);
    }
}
