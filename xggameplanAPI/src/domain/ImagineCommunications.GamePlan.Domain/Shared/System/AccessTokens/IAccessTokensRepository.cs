using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens
{
    public interface IAccessTokensRepository
    {
        AccessToken Find(string token);

        void Delete(string token);

        void Insert(AccessToken accessToken);

        IEnumerable<AccessToken> RemoveAllExpired();

        void SaveChanges();
    }
}
