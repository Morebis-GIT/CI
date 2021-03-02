using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ClearanceDomainModelHandler : IDomainModelHandler<ClearanceCode>
    {
        private readonly IClearanceRepository _clearanceRepository;

        public ClearanceDomainModelHandler(IClearanceRepository clearanceRepository) => _clearanceRepository =
            clearanceRepository ?? throw new ArgumentNullException(nameof(clearanceRepository));

        public ClearanceCode Add(ClearanceCode model)
        {
            _clearanceRepository.Add(model);
            return model;
        }

        public void AddRange(params ClearanceCode[] models) => _clearanceRepository.Add(models);

        public int Count() => _clearanceRepository.CountAll;

        public void DeleteAll() => _clearanceRepository.Truncate();

        public IEnumerable<ClearanceCode> GetAll() => _clearanceRepository.GetAll();
    }
}
