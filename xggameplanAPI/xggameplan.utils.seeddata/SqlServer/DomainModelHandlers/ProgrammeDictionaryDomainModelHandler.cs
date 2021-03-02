using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ProgrammeDictionaryEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeDictionary;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ProgrammeDictionaryDomainModelHandler : IDomainModelHandler<ProgrammeDictionary>
    {
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProgrammeDictionaryRepository _programmeDictionaryRepository;

        public ProgrammeDictionaryDomainModelHandler(ISqlServerDbContext dbContext, IMapper mapper,
            IProgrammeDictionaryRepository programmeDictionaryRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _programmeDictionaryRepository = programmeDictionaryRepository ??
                                             throw new ArgumentNullException(nameof(programmeDictionaryRepository));
        }

        public ProgrammeDictionary Add(ProgrammeDictionary model)
        {
            _ = _dbContext.Add(_mapper.Map<ProgrammeDictionaryEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params ProgrammeDictionary[] models) =>
            _dbContext.BulkInsertEngine.BulkInsert(_mapper.Map<List<ProgrammeDictionaryEntity>>(models),
                post => post.TryToUpdate(models), _mapper);

        public int Count() => _programmeDictionaryRepository.CountAll;

        public void DeleteAll() => _dbContext.Truncate<ProgrammeDictionaryEntity>();

        public IEnumerable<ProgrammeDictionary> GetAll() => _programmeDictionaryRepository.GetAll();
    }
}
