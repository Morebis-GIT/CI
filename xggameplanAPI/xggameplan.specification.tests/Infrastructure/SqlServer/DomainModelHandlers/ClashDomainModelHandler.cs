using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using xggameplan.specification.tests.Interfaces;
using ClashEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Clash;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class ClashDomainModelHandler : SimpleDomainModelMappingHandler<ClashEntity, Clash>
    {
        private readonly IClashRepository _repository;

        public ClashDomainModelHandler(IClashRepository repository, ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _repository = repository;
        }

        public override Clash Add(Clash model)
        {
            _repository.Add(model);
            return model;
        }

        public override void AddRange(params Clash[] models)
        {
            _repository.Add(models);
        }

        public override IEnumerable<Clash> GetAll() => _repository.GetAll();
    }
}
