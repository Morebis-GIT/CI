using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using xggameplan.specification.tests.Interfaces;
using RestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class RestrictionDomainModelHandler : SimpleDomainModelMappingHandler<RestrictionEntity, Restriction>
    {
        private readonly IRestrictionRepository _repository;

        public RestrictionDomainModelHandler(IRestrictionRepository repository, ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _repository = repository;
        }

        public override Restriction Add(Restriction model)
        {
            _repository.Add(new[] {model});
            return model;
        }

        public override void AddRange(params Restriction[] models)
        {
            _repository.Add(models);
        }

        public override IEnumerable<Restriction> GetAll() => _repository.GetAll();
    }
}
