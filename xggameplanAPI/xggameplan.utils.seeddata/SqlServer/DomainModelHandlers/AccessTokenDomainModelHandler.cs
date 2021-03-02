using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AccessTokenDomainModelHandler : IDomainModelHandler<AccessToken>
    {
        private readonly IAccessTokensRepository _accessTokenRepository;
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly IMapper _mapper;

        public AccessTokenDomainModelHandler(
            IAccessTokensRepository accessTokenRepository,
            ISqlServerMasterDbContext dbContext,
            IMapper mapper)
        {
            _accessTokenRepository = accessTokenRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public AccessToken Add(AccessToken model)
        {
            _accessTokenRepository.Insert(model);
            return model;
        }

        public void AddRange(params AccessToken[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<MasterEntities.AccessToken>().Count();

        public void DeleteAll() => _dbContext.Truncate<MasterEntities.AccessToken>();

        public IEnumerable<AccessToken> GetAll() => _dbContext.Query<MasterEntities.AccessToken>().ProjectTo<AccessToken>(_mapper.ConfigurationProvider);
    }
}
