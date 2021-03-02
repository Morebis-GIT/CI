using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;

namespace ImagineCommunications.GamePlan.Domain.Shared.Programmes
{
    public interface IProgrammeRepository
        : IRepository<Programme>
    {
        void Delete(Guid uid);

        void DeleteRange(IEnumerable<Guid> ids);

        Programme Get(Guid uid);

        IEnumerable<Programme> Search(DateTime datefrom, DateTime dateto, string salesarea);

        PagedQueryResult<ProgrammeNameModel> Search(ProgrammeSearchQueryModel searchQuery);

        void SaveChanges();

        Task TruncateAsync();

        bool Exists(Expression<Func<Programme, bool>> condition);
    }
}
