using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes
{
    public interface IClashRepository
        : IRepository<Clash>
    {
        PagedQueryResult<ClashNameModel> Search(ClashSearchQueryModel queryModel);

        IEnumerable<ClashNameModel> GetDescriptionByExternalRefs(ICollection<string> externalRefs);

        /// <summary>
        /// Returns the count of all Clash objects.
        /// </summary>
        /// <returns></returns>
        int Count();

        void Delete(Guid uid);

        void DeleteRange(IEnumerable<Guid> ids);

#pragma warning disable CA1716 // Identifiers should not match keywords
        Clash Get(Guid uid);
#pragma warning restore CA1716 // Identifiers should not match keywords

        void UpdateRange(IEnumerable<Clash> clashes);

        [Obsolete("Use Delete()")]
        void Remove(Guid id, out bool isDeleted);

        void SaveChanges();

        Task TruncateAsync();
    }
}
