using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces
{
    public interface IScenarioDbContext : IDomainModelContext
    {
        void WaitForIndexesAfterSaveChanges();

        void WaitForIndexesToBeFresh();

        void SaveChanges();

        void Cleanup();
    }
}
