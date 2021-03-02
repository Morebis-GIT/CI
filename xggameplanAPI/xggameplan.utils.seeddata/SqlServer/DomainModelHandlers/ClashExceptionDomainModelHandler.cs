using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ClashExceptionDomainModelHandler : IDomainModelHandler<ClashException>
    {
        private readonly IClashExceptionRepository _clashExceptionRepository;

        public ClashExceptionDomainModelHandler(IClashExceptionRepository clashExceptionRepository) =>
            _clashExceptionRepository = clashExceptionRepository ??
                                        throw new ArgumentNullException(nameof(clashExceptionRepository));

        public ClashException Add(ClashException model)
        {
            _clashExceptionRepository.Add(model);
            return model;
        }

        public void AddRange(params ClashException[] models) => _clashExceptionRepository.Add(models);

        public int Count() => _clashExceptionRepository.CountAll;

        public void DeleteAll() => _clashExceptionRepository.Truncate();

        public IEnumerable<ClashException> GetAll() => _clashExceptionRepository.GetAll();
    }
}
