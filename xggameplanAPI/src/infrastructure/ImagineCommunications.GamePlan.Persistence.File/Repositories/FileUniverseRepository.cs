using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileUniverseRepository : FileRepositoryBase, IUniverseRepository
    {
        public FileUniverseRepository(string folder) : base(folder, "universe")
        {
        }

        public void Dispose()
        {
            DeleteAllItems(_folder, _type);
        }

        public IEnumerable<Universe> GetAll()
        {
            return GetAllByType<Universe>(_folder, _type);
        }

        public IEnumerable<Universe> GetBySalesAreaDemo(string salesarea, string demographic)
        {
            return GetAllByType<Universe>(_folder, _type, u => u.SalesArea == salesarea && u.Demographic == demographic).OrderByDescending(u => u.EndDate).ToList();
        }

        public IEnumerable<Universe> Search(List<string> demographics, List<string> salesAreas, DateTime startDate, DateTime endDate)
        {
            //Demo code and / or SalesArea and / or DateRange(if no parameters included, return ALL)
            return GetAllByType<Universe>(_folder, _type,
                u => (salesAreas == null || salesAreas.Count == 0 || salesAreas.Contains(u.SalesArea)) &&
                (demographics == null || demographics.Count == 0 || demographics.Contains(u.Demographic)) &&
                (startDate.Date == DateTime.MinValue || u.StartDate.Date >= startDate.Date) &&
                (endDate.Date == DateTime.MinValue || u.EndDate.Date < endDate.Date.AddDays(1)));
        }

        public Universe Find(Guid id)
        {
            return GetItemByID<Universe>(_folder, _type, id.ToString());
        }

        public void Insert(IEnumerable<Universe> universes)
        {
            InsertItems(_folder, _type, universes.ToList(), universes.Select(i => i.Id.ToString()).ToList());
        }

        public void Update(Universe universe)
        {
            UpdateOrInsertItem(_folder, _type, universe, universe.Id.ToString());
        }

        public void Remove(Guid id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems(_folder, _type);
        }

        public void DeleteByCombination(string salesArea, string demographic, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
