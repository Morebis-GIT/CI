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

        public void Dispose() { }

        public void Insert(IEnumerable<SpotPlacement> spotPlacements)
        {
            InsertItems(spotPlacements.ToList(), spotPlacements.Select(i => i.Id.ToString()).ToList<string>());
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
            UpdateOrInsertItem(spotPlacement, spotPlacement.Id.ToString());
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

        public void SaveChanges() { }

        public void Add(SpotPlacement item)
        {
            UpdateOrInsertItem(item, item.Id.ToString());
        }
    }
}
