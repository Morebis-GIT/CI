using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SpotPlacements
{
    /// <summary>
    /// Interface for Spot Placement repository
    /// </summary>
    public interface ISpotPlacementRepository
    {
        void Add(SpotPlacement item);

        void Insert(IEnumerable<SpotPlacement> spotPlacements);

        SpotPlacement GetByExternalSpotRef(string externalSpotRef);

        List<SpotPlacement> GetByExternalSpotRefs(IEnumerable<string> externalSpotRefs);

        void Update(SpotPlacement spotPlacement);

        void Delete(int id);

        void Delete(string externalSpotRef);

        void DeleteBefore(DateTime modifiedTime);

        void SaveChanges();
    }
}
