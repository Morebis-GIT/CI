using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileSpotPlacementRepository : FileRepositoryBase, ISpotPlacementRepository
    {
        public FileSpotPlacementRepository(string folder) : base(folder, "spot_placement")
        {
        }

        public void Dispose()
        {
        }

        public void Insert(IEnumerable<SpotPlacement> spotPlacements)
        {
            InsertItems(_folder, _type, spotPlacements.ToList(), spotPlacements.Select(i => i.Id.ToString()).ToList());
        }

        public SpotPlacement GetByExternalSpotRef(string externalSpotRef)
        {
            return GetAllByType<SpotPlacement>(_folder, _type, sp => sp.ExternalSpotRef == externalSpotRef).FirstOrDefault();
        }

        public List<SpotPlacement> GetByExternalSpotRefs(IEnumerable<string> externalSpotRefs)
        {
            return GetAllByType<SpotPlacement>(_folder, _type, sp => externalSpotRefs.Contains(sp.ExternalSpotRef));
        }

        public void Update(SpotPlacement spotPlacement)
        {
            UpdateOrInsertItem(_folder, _type, spotPlacement, spotPlacement.Id.ToString());
        }

        public void Delete(int id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        public void Delete(string externalSpotRef)
        {
            DeleteAllItems<SpotPlacement>(_folder, _type, sp => sp.ExternalSpotRef == externalSpotRef);
        }

        public void DeleteBefore(DateTime modifiedTime)
        {
            DeleteAllItems<SpotPlacement>(_folder, _type, sp => sp.ModifiedTime < modifiedTime);
        }

        public void SaveChanges()
        {
        }

        public void Add(SpotPlacement item)
        {
            UpdateOrInsertItem(_folder, _type, item, item.Id.ToString());
        }
    }
}
