using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.Facilities
{
    public interface IFacilityRepository
    {
        void Add(Facility facility);

        void Delete(int id);

        Facility Get(int id);

        Facility GetByCode(string code);

        IEnumerable<Facility> GetAll();

        void SaveChanges();

        void Update(Facility facility);
    }
}
