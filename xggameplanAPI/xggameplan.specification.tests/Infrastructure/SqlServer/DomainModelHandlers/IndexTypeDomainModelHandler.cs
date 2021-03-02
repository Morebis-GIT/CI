using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using xggameplan.specification.tests.Interfaces;
using IndexTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.IndexType;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class IndexTypeDomainModelHandler : SimpleDomainModelMappingHandler<IndexTypeEntity, IndexType>
    {
        private readonly IIndexTypeRepository _repository;

        public IndexTypeDomainModelHandler(IIndexTypeRepository repository, ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _repository = repository;
        }

        public override IndexType Add(IndexType model)
        {
            _repository.Add(new[] {model});
            return model;
        }

        public override void AddRange(params IndexType[] models)
        {
            _repository.Add(models);
        }

        public override IEnumerable<IndexType> GetAll() => _repository.GetAll();
    }
}
