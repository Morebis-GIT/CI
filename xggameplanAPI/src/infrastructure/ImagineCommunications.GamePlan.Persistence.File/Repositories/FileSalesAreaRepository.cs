using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileSalesAreaRepository
        : FileRepositoryBase, ISalesAreaRepository
    {
        public FileSalesAreaRepository(string folder)
            : base(folder, "sales_area")
        {
        }

        public void Dispose()
        {
        }

        public SalesArea Find(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SalesArea> GetAll()
        {
            return GetAllByType<SalesArea>(_folder, _type);
        }

        public void Add(SalesArea salesArea)
        {
            List<SalesArea> items = new List<SalesArea>() { salesArea };
            InsertItems(_folder, _type, items, items.ConvertAll(i => i.Id.ToString()));
        }

        public void Update(SalesArea salesArea)
        {
            UpdateOrInsertItem(_folder, _type, salesArea, salesArea.Id.ToString());
        }

        public void Update(List<SalesArea> salesAreas)
        {
            salesAreas.ForEach(salesArea => UpdateOrInsertItem(_folder, _type, salesArea, salesArea.Id.ToString()));
        }

        public void Remove(Guid id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        /// <summary>
        /// Get the sales area by names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<SalesArea> FindByNames(List<string> names)
        {
            return GetAllByType<SalesArea>(_folder, _type, sa => names.Contains(sa.Name));
        }

        public SalesArea FindByCustomId(int id) =>
            GetAllByType<SalesArea>(_folder, _type, s => s.CustomId == id).FirstOrDefault();

        public SalesArea FindByName(string name)
        {
            var allSalesAreas = GetAllByType<SalesArea>(_folder, _type).ToList();
            var result = allSalesAreas.Find(sa => sa.Name.ToUpper() == name.ToUpper());

            return GetAllByType<SalesArea>(_folder, _type, _ => true).FirstOrDefault();
        }

        public List<SalesArea> FindByIds(List<int> Ids)
        {
            return GetAllByType<SalesArea>(_folder, _type, s => Ids.Contains(s.CustomId)).ToList();
        }

        public List<string> GetListOfNames(List<SalesArea> salesAreas) => throw new NotImplementedException();

        public List<string> GetListOfNames()
        {
            throw new NotImplementedException();
        }

        public int CountAll => GetAllByType<SalesArea>(_folder, _type).Count;

        public void SaveChanges()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }
    }
}
