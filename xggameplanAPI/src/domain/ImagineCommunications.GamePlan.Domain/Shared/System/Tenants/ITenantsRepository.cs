using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Tenants
{
    public interface ITenantsRepository: IPreviewFileStorage
    {
        Tenant GetById(int id);

        List<Tenant> GetAll();

        void Add(Tenant tenant);
        void Update(Tenant tenant);

        void SaveChanges();
    }
}
