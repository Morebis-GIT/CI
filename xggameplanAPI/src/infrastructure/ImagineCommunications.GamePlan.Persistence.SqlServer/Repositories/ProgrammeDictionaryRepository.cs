using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ProgrammeDictionaryRepository : IProgrammeDictionaryRepository
    {
        private const int MaxClauseCount = 2000;

        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProgrammeDictionaryRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.ProgrammeDictionary>().Count();

        public ProgrammeDictionary Find(int id) =>
            _mapper.Map<ProgrammeDictionary>(_dbContext.Find<Entities.Tenant.ProgrammeDictionary>(id));

        public IEnumerable<ProgrammeDictionary> FindByExternal(List<string> externalRefs)
        {
            var products = new List<ProgrammeDictionary>();
            externalRefs = externalRefs.Distinct().ToList();

            for (int i = 0; i <= externalRefs.Count / MaxClauseCount; i++)
            {
                var programmeNames = externalRefs.Skip(i * MaxClauseCount).Take(MaxClauseCount).ToArray();
                products.AddRange(_dbContext.Query<Entities.Tenant.ProgrammeDictionary>()
                    .Where(x => programmeNames.Contains(x.ExternalReference)).ProjectTo<ProgrammeDictionary>(_mapper.ConfigurationProvider).ToArray());
            }

            return products;
        }

        public IEnumerable<ProgrammeDictionary> GetAll() =>
            _dbContext.Query<Entities.Tenant.ProgrammeDictionary>().ProjectTo<ProgrammeDictionary>(_mapper.ConfigurationProvider).ToList();
    }
}
