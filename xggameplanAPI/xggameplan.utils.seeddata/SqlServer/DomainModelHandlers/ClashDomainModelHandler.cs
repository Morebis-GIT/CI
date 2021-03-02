using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ClashDomainModelHandler : IDomainModelHandler<Clash>
    {
        private readonly IClashRepository _clashRepository;

        public ClashDomainModelHandler(IClashRepository clashRepository) =>
            _clashRepository = clashRepository ?? throw new ArgumentNullException(nameof(clashRepository));

        public Clash Add(Clash model)
        {
            _clashRepository.Add(model);
            return model;
        }
        public void AddRange(params Clash[] models) => _clashRepository.Add(models);

        public int Count() => _clashRepository.CountAll;

        public void DeleteAll() => _clashRepository.Truncate();

        public IEnumerable<Clash> GetAll() => _clashRepository.GetAll();
    }
}
