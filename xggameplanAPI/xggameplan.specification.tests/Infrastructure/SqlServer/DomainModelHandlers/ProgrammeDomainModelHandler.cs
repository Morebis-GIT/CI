using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using xggameplan.specification.tests.Interfaces;
using ProgrammeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.Programme;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    /// <summary>
    /// Programme entity is too complicated and programme repository add/addrange methods are used in order to populate data
    /// </summary>
    public class ProgrammeDomainModelHandler : SimpleDomainModelMappingHandler<ProgrammeEntity, Programme>
    {
        private readonly IProgrammeRepository _repository;

        public ProgrammeDomainModelHandler(IProgrammeRepository repository, ISqlServerTestDbContext dbContext,
            IMapper mapper) : base(dbContext, mapper)
        {
            _repository = repository;
        }

        public override Programme Add(Programme model)
        {
            _repository.Add(model);
            return model;
        }

        public override void AddRange(params Programme[] models)
        {
            _repository.Add(models);
        }

        public override IEnumerable<Programme> GetAll() => _repository.GetAll();
    }
}
