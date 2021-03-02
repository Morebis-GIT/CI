using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileDemographicRepository
        : FileRepositoryBase, IDemographicRepository
    {
        public FileDemographicRepository(string folder)
            : base(folder, "demographic")
        {

        }

        public void Dispose()
        {

        }

        public void Add(IEnumerable<Demographic> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList());
        }

        public void Add(Demographic item)
        {
            List<Demographic> items = new List<Demographic>() { item };
            InsertItems(_folder, _type, items, items.Select(i => i.Id.ToString()).ToList());
        }

        public Demographic GetByExternalRef(string externalRef)
        {
            return GetAllByType<Demographic>(_folder, _type, d => d.ExternalRef == externalRef).FirstOrDefault();
        }

        public Demographic GetById(int id)
        {
            return GetItemByID<Demographic>(_folder, _type, id.ToString());
        }

        public void Delete(int id)
        {
            DeleteItem<Demographic>(_folder, _type, id.ToString());
        }

        public IEnumerable<Demographic> GetAll()
        {
            return GetAllByType<Demographic>(_folder, _type);
        }

        public int CountAll
        {
            get
            {
                return CountAll<Demographic>(_folder, _type);
            }
        }

        public void Update(Demographic demographic)
        {
            UpdateOrInsertItem(_folder, _type, demographic, demographic.Id.ToString());
        }

        public void UpdateRange(IEnumerable<Demographic> demographics)
        {
            Add(demographics);
        }

        public void Truncate()
        {
            DeleteAllItems<Demographic>(_folder, _type);
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(IEnumerable<Demographic> items) => throw new NotImplementedException();

        public void SaveChanges()
        {

        }

        public List<string> GetAllGameplanDemographics() => throw new NotImplementedException();

        public IEnumerable<Demographic> GetByExternalRef(List<string> externalRefs) => throw new NotImplementedException();
    }
}
