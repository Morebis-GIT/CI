using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryUniverseRepository :
        MemoryRepositoryBase<Universe>,
        IUniverseRepository
    {
        public MemoryUniverseRepository()
        {
        }

        public void Dispose()
        {
        }

        public IEnumerable<Universe> GetAll() => GetAllItems();

        public IEnumerable<Universe> GetBySalesAreaDemo(string salesarea, string demographic)
        {
            return GetAllItems(u => u.SalesArea == salesarea && u.Demographic == demographic);
        }

        public IEnumerable<Universe> Search(List<string> demographics, List<string> salesAreas, DateTime startDate, DateTime endDate)
        {
            //Demo code and / or SalesArea and / or DateRange(if no parameters included, return ALL)
            return GetAllItems(u => (salesAreas == null || salesAreas.Count == 0 || salesAreas.Contains(u.SalesArea)) &&
                (demographics == null || demographics.Count == 0 || demographics.Contains(u.Demographic)) &&
                (startDate.Date == DateTime.MinValue || u.StartDate.Date >= startDate.Date) &&
                (endDate.Date == DateTime.MinValue || u.EndDate.Date < endDate.Date.AddDays(1)));
        }

        public Universe Find(Guid id) => GetItemById(id.ToString());

        public void Insert(IEnumerable<Universe> items)
        {
            foreach (var item in items)
            {
                InsertOrReplaceItem(item, item.Id.ToString());
            }
        }

        public void Update(Universe universe)
        {
            InsertOrReplaceItem(universe, universe.Id.ToString());
        }

        public void Remove(Guid id)
        {
            DeleteItem(id.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public void DeleteByCombination(string salesArea, string demographic, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
        }
    }
}
