using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ProgrammeDomainModelHandler : IDomainModelHandler<Programme>
    {
        private readonly IProgrammeRepository _programmeRepository;

        public ProgrammeDomainModelHandler(IProgrammeRepository programmeRepository) =>
            _programmeRepository = programmeRepository ?? throw new ArgumentNullException(nameof(programmeRepository));

        public Programme Add(Programme model)
        {
            _programmeRepository.Add(model);
            return model;
        }

        public void AddRange(params Programme[] models) => _programmeRepository.Add(models);

        public int Count() => _programmeRepository.CountAll;

        public void DeleteAll() => _programmeRepository.Truncate();

        public IEnumerable<Programme> GetAll() => _programmeRepository.GetAll();
    }
}
