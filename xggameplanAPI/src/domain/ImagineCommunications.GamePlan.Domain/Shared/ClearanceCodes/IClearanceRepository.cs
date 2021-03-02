using ImagineCommunications.GamePlan.Domain.Generic.Repository;

namespace ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes
{
    public interface IClearanceRepository : IRepository<ClearanceCode>
    {
        ClearanceCode Find(int id);

        void Remove(int id, out bool isDeleted);

        void SaveChanges();
    }
}
