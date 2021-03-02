using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.common.Caching;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AccessTokenRepository : IAccessTokensRepository
    {
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly ICache _cache;
        private readonly IMapper _mapper;

        public AccessTokenRepository(ISqlServerMasterDbContext dbContext, ICache cache, IMapper mapper)
        {
            _dbContext = dbContext;
            _cache = cache;
            _mapper = mapper;
        }

        public void Delete(string accessToken)
        {
            var entityToDelete = _dbContext.Query<Entities.Master.AccessToken>()
                .FirstOrDefault(e => e.Token == accessToken);
            // non-expiring token rule
            if (entityToDelete?.ValidUntilValue.Year < DateTime.Today.Year + 5)
            {
                _dbContext.Remove(entityToDelete);
            }
        }
        public AccessToken Find(string accessToken) => GetToken(accessToken);

        public void Insert(AccessToken accessToken)
        {
            var accessTokenEntity = _mapper.Map<Entities.Master.AccessToken>(accessToken);
            _dbContext.Add(accessTokenEntity,
                post =>
                {
                    _cache.Add(accessToken.Token, accessToken, TimeSpan.FromMinutes(20));
                    post.MapTo(accessToken);
                }, _mapper);
        }

        public IEnumerable<AccessToken> RemoveAllExpired()
        {
            var expiredEntities = _dbContext.Query<Entities.Master.AccessToken>().Where(ent => ent.ValidUntilValue < DateTime.UtcNow).ToArray();
            if (!expiredEntities.Any())
            {
                return Array.Empty<AccessToken>();
            }

            _dbContext.RemoveRange(expiredEntities);
            foreach (var entity in expiredEntities)
            {
                _cache.Remove(entity.Token);
            }

            return _mapper.Map<List<AccessToken>>(expiredEntities);
        }

        private AccessToken GetToken(string accessToken)
        {
            var cachedToken = _cache.Get<AccessToken>(accessToken);
            if (cachedToken != null)
            {
                return cachedToken;
            }

            var tokenEntity = _dbContext.Query<Entities.Master.AccessToken>()
                .Where(e => e.Token == accessToken)
                .ProjectTo<AccessToken>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

            if (tokenEntity != null)
            {
                _cache.Add(tokenEntity.Token, tokenEntity, TimeSpan.FromMinutes(20));
            }
            return tokenEntity;
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
