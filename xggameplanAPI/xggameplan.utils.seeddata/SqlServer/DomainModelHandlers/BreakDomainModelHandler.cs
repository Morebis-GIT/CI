using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class BreakDomainModelHandler : IDomainModelHandler<Break>
    {
        private readonly IBreakRepository _breakRepository;

        public BreakDomainModelHandler(IBreakRepository breakRepository) =>
            _breakRepository = breakRepository ?? throw new ArgumentNullException(nameof(breakRepository));

        public Break Add(Break model)
        {
            _breakRepository.Add(model);
            return model;
        }

        public void AddRange(params Break[] models) => _breakRepository.Add(models);

        public int Count() => _breakRepository.CountAll;

        public void DeleteAll() => _breakRepository.Truncate();

        public IEnumerable<Break> GetAll() => _breakRepository.GetAll();
    }
}
