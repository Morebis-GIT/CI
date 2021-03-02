using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT17122
{
    internal class UpdateStepXGGT17122_NullPassStartEndTime : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionString;

        public UpdateStepXGGT17122_NullPassStartEndTime(IEnumerable<string> tenantConnectionString, string updatesFolder)
        {
            _tenantConnectionString = tenantConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("9e9e26fe-2c42-4fc7-bc0d-6b0e03f052e2");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionString)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    IPassRepository passRepository = new RavenPassRepository(session);

                    SetSalesAreaPriorities(passRepository);
                    SetSalesAreaPrioritiesStartEndTime(passRepository);
                    SetSalesAreaPrioritiesDayOfWeek(passRepository);

                    passRepository.SaveChanges();
                }
            }
        }

        private void SetSalesAreaPriorities(IPassRepository passRepository)
        {
            IEnumerable<Pass> passesWithNullSalesAreaTimes = passRepository
                .GetAll()
                .Where(p => p.PassSalesAreaPriorities is null);

            foreach (var pass in passesWithNullSalesAreaTimes)
            {
                pass.PassSalesAreaPriorities = new PassSalesAreaPriority();
                passRepository.Update(pass);
            }
        }

        private void SetSalesAreaPrioritiesStartEndTime(IPassRepository passRepository)
        {
            IEnumerable<Pass> passesWithNullSalesAreaTimes = passRepository
                .GetAll()
                .Where(p => (
                    p.PassSalesAreaPriorities.StartTime is null ||
                    p.PassSalesAreaPriorities.EndTime is null));

            foreach (var pass in passesWithNullSalesAreaTimes)
            {
                pass.PassSalesAreaPriorities.StartTime = new TimeSpan(6, 0, 0);
                pass.PassSalesAreaPriorities.EndTime = new TimeSpan(5, 59, 59);
                passRepository.Update(pass);
            }
        }

        private void SetSalesAreaPrioritiesDayOfWeek(IPassRepository passRepository)
        {
            IEnumerable<Pass> passesWithNullDayOfWeek = passRepository
                .GetAll()
                .Where(p => string.IsNullOrWhiteSpace(p.PassSalesAreaPriorities.DaysOfWeek));

            foreach (var pass in passesWithNullDayOfWeek)
            {
                pass.PassSalesAreaPriorities.DaysOfWeek = "0000000";
                passRepository.Update(pass);
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-17122";

        public bool SupportsRollback => false;
    }
}
