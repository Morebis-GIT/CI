using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenBusinessTypeRepository : IBusinessTypeRepository
    {
        private readonly IDocumentSession _session;

        public RavenBusinessTypeRepository(IDocumentSession session)
            => _session = session;

        public IEnumerable<BusinessType> GetAll() => _session.GetAll<BusinessType>();

        public BusinessType GetByCode(string code) =>
            _session.Query<BusinessType>()
            .FirstOrDefault(b => b.Code == code);

        public bool Exists(string code) =>
             _session.Query<BusinessType>().Any(b => b.Code == code);
    }
}
