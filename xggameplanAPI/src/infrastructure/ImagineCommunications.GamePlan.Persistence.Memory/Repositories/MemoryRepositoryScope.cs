using System;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    /// <summary>
    /// Factory for memory repositories
    /// </summary>
    public class MemoryRepositoryScope : IRepositoryScope
    {
        static MemoryRepositoryScope() => _repositoryDetails = CreateRepositoryDetails();

        public MemoryRepositoryScope(Guid repositorySegmentationSalt) =>
            _repositorySegmentationSalt = repositorySegmentationSalt;

        public MemoryRepositoryScope(Guid repositorySegmentationSalt, double objectTTL) =>
            (_repositorySegmentationSalt, _objectTTL) = (repositorySegmentationSalt, objectTTL);

        private static RepositoryDetailInfo[] CreateRepositoryDetails()
        {
            return new RepositoryDetailInfo[]
            {
                new RepositoryDetailInfo(typeof(IBreakRepository), typeof(MemoryBreakRepository)),
                new RepositoryDetailInfo(typeof(ICampaignRepository), typeof(MemoryCampaignRepository)),
                new RepositoryDetailInfo(typeof(IClashExceptionRepository), typeof(MemoryClashExceptionRepository)),
                new RepositoryDetailInfo(typeof(IClashRepository), typeof(MemoryClashRepository)),
                new RepositoryDetailInfo(typeof(IDemographicRepository), typeof(MemoryDemographicRepository)),
                new RepositoryDetailInfo(typeof(IIndexTypeRepository), typeof(MemoryIndexTypeRepository)),
                new RepositoryDetailInfo(typeof(IProductRepository), typeof(MemoryProductRepository)),
                new RepositoryDetailInfo(typeof(IProgrammeRepository), typeof(MemoryProgrammeRepository)),
                new RepositoryDetailInfo(typeof(IRatingsScheduleRepository), typeof(MemoryRatingsScheduleRepository)),
                new RepositoryDetailInfo(typeof(IRecommendationRepository), typeof(MemoryRecommendationRepository)),
                new RepositoryDetailInfo(typeof(IRestrictionRepository), typeof(MemoryRestrictionRepository)),
                new RepositoryDetailInfo(typeof(ISalesAreaRepository), typeof(MemorySalesAreaRepository)),
                new RepositoryDetailInfo(typeof(IScheduleRepository), typeof(MemoryScheduleRepository)),
                new RepositoryDetailInfo(typeof(ISmoothConfigurationRepository), typeof(MemorySmoothConfigurationRepository)),
                new RepositoryDetailInfo(typeof(ISmoothFailureMessageRepository), typeof(MemorySmoothFailureMessageRepository)),
                new RepositoryDetailInfo(typeof(ISmoothFailureRepository), typeof(MemorySmoothFailureRepository)),
                new RepositoryDetailInfo(typeof(ISponsorshipRepository), typeof(MemorySponsorshipRepository)),
                new RepositoryDetailInfo(typeof(ISpotPlacementRepository), typeof(MemorySpotPlacementRepository)),
                new RepositoryDetailInfo(typeof(ISpotRepository), typeof(MemorySpotRepository)),
                new RepositoryDetailInfo(typeof(ITenantSettingsRepository), typeof(MemoryTenantSettingsRepository)),
                new RepositoryDetailInfo(typeof(IUniverseRepository), typeof(MemoryUniverseRepository)),
            };
        }

        private static readonly RepositoryDetailInfo[] _repositoryDetails;
        private readonly Guid _repositorySegmentationSalt;
        private readonly double _objectTTL;

        /// <summary>
        /// Details of a repository
        /// </summary>
        private class RepositoryDetailInfo
        {
            public Type InterfaceType { get; }
            public Type ImplementationType { get; }

            public RepositoryDetailInfo(Type interfaceType, Type implementationType)
            {
                InterfaceType = interfaceType;
                ImplementationType = implementationType;
            }
        }

        /// <summary>
        /// Creates an instance of a repository of type <typeparamref name="TRepository"/>.
        /// </summary>
        /// <typeparam name="TRepository">The type of repository to create.</typeparam>
        /// <returns>Returns an instances of the repository type.</returns>
        public TRepository CreateRepository<TRepository>() where TRepository : class =>
            CreateRepositories(typeof(TRepository)).Get<TRepository>();

        public IRepositoryScope BeginRepositoryScope() => this;

        /// <summary>
        /// Creates repository instances
        /// </summary>
        /// <param name="repositoryTypes"></param>
        /// <returns></returns>
        public RepositoryDictionary CreateRepositories(params Type[] repositoryTypes)
        {
            var repositories = new RepositoryDictionary();

            foreach (Type repositoryType in repositoryTypes)
            {
                var repositoryDetails = Array.Find(_repositoryDetails, ri =>
                    ri.InterfaceType == repositoryType || ri.ImplementationType == repositoryType);

                if (repositoryDetails is null)
                {
                    throw new NotImplementedException($"Unable to create repository {repositoryType.Name}");
                }

                repositories.Add(repositoryType, CreateRepository(repositoryDetails));
            }

            return repositories;
        }

        /// <summary>
        /// Creates a repository of the specified type.
        /// </summary>
        /// <param name="repositoryDetails"></param>
        /// <returns></returns>
        private object CreateRepository(RepositoryDetailInfo repositoryDetails)
        {
            var result = Activator.CreateInstance(repositoryDetails.ImplementationType);

            if (result is IMemoryRepositoryCompartment objectWithSalt)
            {
                objectWithSalt.CompartmentKey = _repositorySegmentationSalt.ToString();
            }

            if (result is IRepositoryObjectTTL objectWithTTL)
            {
                objectWithTTL.ObjectTTLMilliseconds = _objectTTL;
            }

            return result;
        }

        public void Dispose()
        {
            //nothing to dispose for memory repository scope implementation
        }
    }
}
