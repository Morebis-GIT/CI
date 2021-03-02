using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public LanguageRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(Language language) =>
            _dbContext.Add(_mapper.Map<Entities.Tenant.Language>(language), post => post.MapTo(language), _mapper);

        public IEnumerable<Language> GetAll() =>
            _dbContext.Query<Entities.Tenant.Language>().ProjectTo<Language>(_mapper.ConfigurationProvider).ToList();
    }
}
