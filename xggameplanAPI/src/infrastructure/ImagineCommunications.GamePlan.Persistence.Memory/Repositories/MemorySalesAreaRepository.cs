using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySalesAreaRepository :
        MemoryRepositoryBase<SalesArea>,
        ISalesAreaRepository
    {
        public MemorySalesAreaRepository()
        {
        }

        public void Dispose()
        {
        }

        public SalesArea Find(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SalesArea> GetAll() => GetAllItems();

        public void Add(SalesArea item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public void Update(SalesArea item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public void Update(List<SalesArea> salesAreas)
        {
            foreach (var salesArea in salesAreas)
            {
                Update(salesArea);
            }
        }

        public void Remove(Guid id)
        {
            DeleteItem(id.ToString());
        }

        /// <summary>
        /// Get the sales area by names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<SalesArea> FindByNames(List<string> names) =>
            GetAllItems(sa => names.Contains(sa.Name));

        public SalesArea FindByCustomId(int id) =>
            GetAllItems().Find(x => x.CustomId.Equals(id));

        public SalesArea FindByName(string name) =>
            GetAllItems().Find(x => x.Name.Equals(name));

        public List<SalesArea> FindByIds(List<int> Ids) =>
            GetAllItems(s => Ids.Contains(s.CustomId)).ToList();

        public List<string> GetListOfNames(List<SalesArea> salesAreas) =>
            salesAreas.Select(item => item.Name).ToList();

        public List<string> GetListOfNames() =>
            GetAllItems().Select(i => i.Name).ToList();

        public int CountAll => GetCount();

        public void SaveChanges()
        {
        }

        public SalesArea FindByShortName(string shortName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SalesArea> FindByShortNames(IEnumerable<string> shortNames)
        {
            throw new NotImplementedException();
        }

        public void DeleteByShortName(string shortName)
        {
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            foreach (var item in GetAllItems().Where(i => ids.Contains(i.Id)))
            {
                DeleteItem(item.Id.ToString());
            }
        }

        public void Truncate()
        {
            DeleteAllItems();
        }
    }
}
