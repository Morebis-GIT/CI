using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using xggameplan.common.Caching;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    /// <summary>
    /// Raven access token repository.

    public class RavenAccessTokensRepository : IAccessTokensRepository
    {
        private readonly IRavenMasterDbContext _dbContext;
        private readonly ICache _cache;

        public RavenAccessTokensRepository(IRavenMasterDbContext dbContext, ICache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public AccessToken Find(string accessToken)
        {
            AccessToken token = _cache == null ? null : _cache.Get<AccessToken>(accessToken);
            if (token == null)    // Not in cache, read Raven
            {
                token = _dbContext.Query<AccessToken>().FirstOrDefault(x => x.Token == accessToken);
                if (token != null)
                {
                    _cache.Add(token.Token, token, new TimeSpan(0, 20, 0));
                }
            }
            return token;
        }

        public void Delete(string accessToken)
        {
            var token = _dbContext.Query<AccessToken>().FirstOrDefault(x => x.Token == accessToken);
            if (token != null && !IsNonExpiring(token))
            {
                _dbContext.Remove(token);
                if (_cache != null)
                {
                    _cache.Remove(accessToken);
                }
            }
        }

        /// <summary>
        /// Removes all expired access tokens
        /// </summary>
        /// <param name="beforeValidUntil"></param>
        /// <returns></returns>
        public IEnumerable<AccessToken> RemoveAllExpired()
        {
            // Get all expired access tokens
            var beforeValidUntil = new DateTimeOffset(DateTime.UtcNow);
            var tokens = _dbContext.Query<AccessToken>()
                            .Where(at => at.ValidUntilValue < beforeValidUntil)
                            .ToList()
                            .Where(at => !IsNonExpiring(at));

            // Delete access token
            foreach (var token in tokens)
            {
                _dbContext.Remove(token);
                if (_cache != null)
                {
                    _cache.Remove(token.Token);
                }
            }
            return tokens;
        }

        private bool IsNonExpiring(AccessToken accessToken) => accessToken.ValidUntilValue.Date.ToUniversalTime() > DateTime.UtcNow.AddYears(5);

        public void Insert(AccessToken accessToken)
        {
            _dbContext.Add(accessToken);
            if (_cache != null)
            {
                _cache.Add(accessToken.Token, accessToken, new TimeSpan(0, 20, 0));
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
