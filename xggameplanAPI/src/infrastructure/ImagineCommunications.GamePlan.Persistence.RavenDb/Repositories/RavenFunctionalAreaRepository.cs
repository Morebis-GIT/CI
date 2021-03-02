using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenFunctionalAreaRepository : IFunctionalAreaRepository
    {
        private readonly IDocumentSession _session;

        public RavenFunctionalAreaRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(FunctionalArea functionalArea)
        {
            lock (_session)
            {
                _session.Store(functionalArea);
            }
        }

        public IEnumerable<FunctionalArea> GetAll()
        {
            lock (_session)
            {
                return _session.Query<FunctionalArea>().Take(int.MaxValue).ToList();
            }
        }

        public IEnumerable<int> GetSelectedFailureTypeIds() => FunctionalAreaFaultTypesTransformerResult(fa => fa.IsSelected)
            .Select(f => f.FaultTypeId)
            .Distinct();

        public FunctionalArea Find(Guid id) => _session.Load<FunctionalArea>(id);

        public FaultType FindFaultType(int faultTypeId) =>
            FunctionalAreaFaultTypesTransformerResult(f => f.FaultTypeId == faultTypeId)
                .Select(fa => new FaultType
                {
                    Id = fa.FaultTypeId,
                    Description = fa.Description,
                    IsSelected = fa.IsSelected
                })
                .FirstOrDefault();

        public IEnumerable<FaultType> FindFaultTypes(List<int> faultTypeIds) =>
            FunctionalAreaFaultTypesTransformerResult(fa => fa.FaultTypeId.In(faultTypeIds))
                .Select(fa => new FaultType
                {
                    Id = fa.FaultTypeId,
                    Description = fa.Description,
                    IsSelected = fa.IsSelected
                });

        public void UpdateFaultTypesSelections(FunctionalArea functionalArea)
        {
            lock (_session)
            {
                _session.Store(functionalArea);
            }
        }

        private IEnumerable<FunctionalAreaFaultTypes_Transformer.Result> FunctionalAreaFaultTypesTransformerResult(
            Func<FunctionalAreaFaultTypes_Transformer.Result, bool> condition) => _session
            .GetAllWithTransform<FunctionalArea, FunctionalAreaFaultTypes_Transformer,
                FunctionalAreaFaultTypes_Transformer.Result>(f => f.Id != Guid.Empty,
                FunctionalAreas_Default.DefaultIndexName, false)
            .Where(condition);
    }
}
