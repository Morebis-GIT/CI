using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ProgrammeClassificationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeClassification;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ProgrammeClassificationDomainModelHandler : IDomainModelHandler<ProgrammeClassification>
    {
        private readonly IProgrammeClassificationRepository _programmeClassificationRepository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProgrammeClassificationDomainModelHandler(
            IProgrammeClassificationRepository programmeClassificationRepository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _programmeClassificationRepository = programmeClassificationRepository ??
                                                 throw new ArgumentNullException(
                                                     nameof(programmeClassificationRepository));
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ProgrammeClassification Add(ProgrammeClassification model)
        {
            _ = _dbContext.Add(_mapper.Map<ProgrammeClassificationEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params ProgrammeClassification[] models) => _programmeClassificationRepository.Add(models);

        public int Count() => _programmeClassificationRepository.CountAll;

        public void DeleteAll() => _programmeClassificationRepository.Truncate();

        public IEnumerable<ProgrammeClassification> GetAll() => _programmeClassificationRepository.GetAll();
    }
}
