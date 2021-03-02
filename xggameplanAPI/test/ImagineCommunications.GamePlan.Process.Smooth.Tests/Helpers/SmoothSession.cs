using System;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.DomainLogic;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    /// <summary>
    /// Smooth session for debugging
    /// </summary>
    public class SmoothSession
    {
        public IClashExposureCountService EffectiveClashExposureCountService { get; }
        public IRepositoryScope RepositoryScope { get; }
        public SalesArea SalesArea { get; }

        public SmoothBatchOutput SmoothBatchOutput { get; set; } = new SmoothBatchOutput();

        public SmoothSession(
            IRepositoryScope repositoryScope,
            string salesAreaName)
        {
            RepositoryScope = repositoryScope;

            EffectiveClashExposureCountService = CreateEffectiveClashExposureCountService(repositoryScope);

            var salesAreaRepository = repositoryScope.CreateRepository<ISalesAreaRepository>();
            SalesArea = salesAreaRepository.FindByName(salesAreaName);

            if (SalesArea is null)
            {
                throw new NullReferenceException(
                    $"Sales area {salesAreaName} was not found in the test data."
                    );
            }
        }

        private static IClashExposureCountService CreateEffectiveClashExposureCountService(
            IRepositoryScope scope)
        {
            TenantSettings tenantSettings = GetTenantSettings(scope);

            if (String.IsNullOrWhiteSpace(tenantSettings.PeakStartTime))
            {
                return ClashExposureCountService.Create();
            }

            return ClashExposureCountService.Create(
                (tenantSettings.PeakStartTime, tenantSettings.PeakEndTime)
                );
        }

        private static TenantSettings GetTenantSettings(
            IRepositoryScope scope)
        {
            var tenantSettingsRepository = scope.CreateRepository<ITenantSettingsRepository>();
            return tenantSettingsRepository.Get();
        }
    }
}
