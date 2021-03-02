using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace xggameplan.specification.tests.Interfaces
{
    public interface IScenarioDbContext : IDomainModelContext
    {
        void WaitForIndexesAfterSaveChanges();

        void WaitForIndexesToBeFresh();

        void SaveChanges();

        void Cleanup();
    }
}
