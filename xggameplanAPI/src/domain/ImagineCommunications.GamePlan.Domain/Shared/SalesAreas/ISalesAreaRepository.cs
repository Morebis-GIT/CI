using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.SalesAreas
{
    public interface ISalesAreaRepository
    {
        SalesArea Find(Guid id);

        IEnumerable<SalesArea> GetAll();

        void Add(SalesArea salesArea);

        void Update(SalesArea salesArea);

        void Remove(Guid id);

        List<SalesArea> FindByNames(List<string> names);

        SalesArea FindByCustomId(int id);

        List<SalesArea> FindByIds(List<int> Ids);

        List<string> GetListOfNames(List<SalesArea> salesAreas);

        List<string> GetListOfNames();

        SalesArea FindByName(string name);

        void Update(List<SalesArea> salesAreas);

        int CountAll { get; }

        void SaveChanges();

        SalesArea FindByShortName(string shortName);

        IEnumerable<SalesArea> FindByShortNames(IEnumerable<string> shortNames);

        void DeleteByShortName(string shortName);

        void DeleteRange(IEnumerable<Guid> ids);
    }
}
