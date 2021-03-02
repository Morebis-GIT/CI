using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySpotPlacementRepository :
        MemoryRepositoryBase<SpotPlacement>,
        ISpotPlacementRepository
    {
        public MemorySpotPlacementRepository()
        {
        }

        public void Dispose()
        {
        }

        public void Add(SpotPlacement item)
        {
            InsertOrReplaceItem(item, item.Id.ToString());
        }

        public void Insert(IEnumerable<SpotPlacement> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public SpotPlacement GetByExternalSpotRef(string externalSpotRef)
        {
            return GetAllItems(sp => sp.ExternalSpotRef == externalSpotRef).FirstOrDefault();
        }

        public List<SpotPlacement> GetByExternalSpotRefs(IEnumerable<string> externalSpotRefs)
        {
            return GetAllItems(sp => externalSpotRefs.Contains(sp.ExternalSpotRef));
        }

        public void Update(SpotPlacement spotPlacement)
        {
            InsertOrReplaceItem(spotPlacement, spotPlacement.Id.ToString());
        }

        public void Delete(int id)
        {
            DeleteItem(id.ToString());
        }

        public void Delete(string externalSpotRef)
        {
            DeleteAllItems(sp => sp.ExternalSpotRef == externalSpotRef);
        }

        public void DeleteBefore(DateTime modifiedTime)
        {
            DeleteAllItems(sp => sp.ModifiedTime < modifiedTime);
        }

        public void SaveChanges()
        {
        }

        public void Truncate()
        {
            DeleteAllItems();
        }
    }
}
