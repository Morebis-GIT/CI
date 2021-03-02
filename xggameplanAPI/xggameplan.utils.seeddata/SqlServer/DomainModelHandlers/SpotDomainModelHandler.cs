using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class SpotDomainModelHandler : IDomainModelHandler<Spot>
    {
        private readonly ISpotRepository _repository;

        public SpotDomainModelHandler(ISpotRepository repository)
        {
            _repository = repository;
        }

        public Spot Add(Spot model)
        {
            _repository.Add(model);

            return model;
        }

        public void AddRange(params Spot[] models) => _repository.Add(models);

        public int Count() => _repository.CountAll;

        public void DeleteAll() => _repository.Truncate();

        public IEnumerable<Spot> GetAll() => _repository.GetAll();
    }
}
