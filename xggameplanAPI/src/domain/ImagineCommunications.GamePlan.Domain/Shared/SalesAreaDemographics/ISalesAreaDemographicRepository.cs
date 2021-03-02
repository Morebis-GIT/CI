using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics
{
    public interface ISalesAreaDemographicRepository
    {
        void AddRange(IEnumerable<SalesAreaDemographic> entities);

        void DeleteBySalesAreaName(string name);

        void DeleteBySalesAreaNames(IEnumerable<string> salesAreaNames);

        IEnumerable<SalesAreaDemographic> GetAll();

        void SaveChanges();
    }
}
