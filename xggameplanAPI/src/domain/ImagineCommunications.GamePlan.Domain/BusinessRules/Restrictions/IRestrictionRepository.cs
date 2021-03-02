using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions
{
    public interface IRestrictionRepository
    {
        void Add(Restriction item);

        [Obsolete("Should be called AddRange() to match .NET conventions")]
        void Add(IEnumerable<Restriction> items);

        IEnumerable<Restriction> GetAll();

        Restriction Get(Guid uid);

        Restriction Get(string externalIdentifier);

        IEnumerable<Restriction> Get(List<string> externalIdentifiers);

        PagedQueryResult<Tuple<Restriction, RestrictionDescription>> Get(RestrictionSearchQueryModel query);

        Tuple<Restriction, RestrictionDescription> GetDesc(Guid id);

        IEnumerable<Restriction> Get(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType);

        void UpdateRange(IEnumerable<Restriction> restrictions);

        void Delete(List<string> salesAreaNames, bool matchAllSpecifiedSalesAreas, DateTime? dateRangeStart, DateTime? dateRangeEnd, RestrictionType? restrictionType);

        void Delete(Guid uid);

        void DeleteRange(IEnumerable<Guid> ids);

        void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs);

        void Truncate();

        void SaveChanges();
    }
}
