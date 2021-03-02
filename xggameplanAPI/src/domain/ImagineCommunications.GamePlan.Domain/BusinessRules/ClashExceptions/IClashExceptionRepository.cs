using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions
{
    public interface IClashExceptionRepository : IRepository<ClashException>
    {
        PagedQueryResult<ClashException> Search(ClashExceptionSearchQueryModel searchQuery);

        PagedQueryResult<ClashExceptionModel> SearchWithDescriptions(ClashExceptionSearchQueryModel searchQuery);

        ClashException Find(int id);

        ClashExceptionModel GetWithDescriptions(int id);

        List<ClashExceptionModel> GetWithDescriptions(IEnumerable<int> ids);

        void Remove(int id);

        void SaveChanges();

        void Delete(ClashExceptionType fromType, ClashExceptionType toType, string fromValue, string toValue);

        IEnumerable<ClashException> GetActive();

        void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs);
    }
}
