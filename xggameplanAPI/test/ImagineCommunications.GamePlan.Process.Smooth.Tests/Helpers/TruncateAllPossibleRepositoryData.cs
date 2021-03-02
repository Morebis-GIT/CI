using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    /// <summary>
    /// Some repositories do not have delete or truncate methods so cannot be truncated.
    /// </summary>
    internal class TruncateAllPossibleRepositoryData
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public TruncateAllPossibleRepositoryData(IRepositoryFactory repositoryFactory) =>
            _repositoryFactory = repositoryFactory;

        public void Truncate()
        {
            TruncateCampaigns();
            TruncateDemographic();
            TruncateIndexType();
            TruncateRatingSchedule();
            TruncateRestriction();
            TruncateSalesArea();
            TruncateSmoothConfiguration();
            TruncateSmoothFailure();
            TruncateSmoothFailureMessage();
            TruncateSponsorship();
            TruncateSpotPlacements();
            TruncateTenantSettings();
            TruncateUniverse();

            Truncate<Break, IBreakRepository>();
            Truncate<Clash, IClashRepository>();
            Truncate<ClashException, IClashExceptionRepository>();
            Truncate<Product, IProductRepository>();
            Truncate<Programme, IProgrammeRepository>();
            Truncate<Spot, ISpotRepository>();
        }

        private void TruncateRatingSchedule()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<IRatingsScheduleRepository>();
            _ = repository.TruncateAsync();
            repository.SaveChanges();
        }

        private void TruncateCampaigns()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ICampaignRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateDemographic()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<IDemographicRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateIndexType()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<IIndexTypeRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateRestriction()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<IRestrictionRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateSalesArea()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ISalesAreaRepository>();

            repository.DeleteRange(
                repository.GetAll().Select(i => i.Id)
                );

            repository.SaveChanges();
        }

        private void TruncateSmoothConfiguration()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ISmoothConfigurationRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateSmoothFailure()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ISmoothFailureRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateSmoothFailureMessage()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ISmoothFailureMessageRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateSponsorship()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ISponsorshipRepository>();
            _ = repository.TruncateAsync();
            repository.SaveChanges();
        }

        private void TruncateSpotPlacements()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ISpotPlacementRepository>();
            repository.DeleteBefore(DateTime.MaxValue);
            repository.SaveChanges();
        }

        private void TruncateTenantSettings()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ITenantSettingsRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void TruncateUniverse()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<IUniverseRepository>();
            repository.Truncate();
            repository.SaveChanges();
        }

        private void Truncate<TModel, TRepository>()
            where TRepository : class, IRepository<TModel>
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<TRepository>();
            repository.Truncate();
        }
    }
}
