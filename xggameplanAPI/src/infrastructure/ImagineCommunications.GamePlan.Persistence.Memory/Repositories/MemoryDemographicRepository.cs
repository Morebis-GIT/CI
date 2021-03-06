﻿using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryDemographicRepository :
        MemoryRepositoryBase<Demographic>,
        IDemographicRepository
    {
        public MemoryDemographicRepository()
        {
        }

        public void Dispose()
        {
        }

        public void Add(IEnumerable<Demographic> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Add(Demographic item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public Demographic GetByExternalRef(string externalRef)
        {
            return GetAllItems(d => d.ExternalRef == externalRef).FirstOrDefault();
        }

        public Demographic GetById(int id)
        {
            return GetItemById(id.ToString());
        }

        public void Delete(int id)
        {
            DeleteItem(id.ToString());
        }

        public IEnumerable<Demographic> GetAll()
        {
            return GetAllItems();
        }

        public int CountAll => GetCount();

        public void Update(Demographic demographic)
        {
            InsertOrReplaceItem(demographic, demographic.Id.ToString());
        }

        public void UpdateRange(IEnumerable<Demographic> demographics)
        {
            Add(demographics);
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            throw new System.NotImplementedException();
        }

        public void InsertOrUpdate(IEnumerable<Demographic> items) => throw new System.NotImplementedException();

        public void SaveChanges()
        {
        }

        public List<string> GetAllGameplanDemographics() =>
            new List<string>(
                GetAllItems()
                    .Where(demo => demo.Gameplan)
                    .Select(d => d.ExternalRef));

        public IEnumerable<Demographic> GetByExternalRef(List<string> externalRefs) =>
            throw new NotImplementedException();
    }
}
