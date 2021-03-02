using System;
using System.IO;
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

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileRepositoryScope : IRepositoryScope
    {
        private static RepositoryDetailInfo[] RepositoryDetails { get; } = new[]
            {
                new RepositoryDetailInfo(typeof(IBreakRepository), typeof(FileBreakRepository), "Breaks"),
                new RepositoryDetailInfo(typeof(ICampaignRepository), typeof(FileCampaignRepository), "Campaigns"),
                new RepositoryDetailInfo(typeof(IClashRepository), typeof(FileClashRepository), "Clashes"),
                new RepositoryDetailInfo(typeof(IClashExceptionRepository), typeof(FileClashExceptionRepository), "ClashExceptions"),
                new RepositoryDetailInfo(typeof(IDemographicRepository), typeof(FileDemographicRepository), "Demographics"),
                new RepositoryDetailInfo(typeof(IIndexTypeRepository), typeof(FileIndexTypeRepository), "IndexTypes"),
                new RepositoryDetailInfo(typeof(IProductRepository), typeof(FileProductRepository), "Products"),
                new RepositoryDetailInfo(typeof(IProgrammeRepository), typeof(FileProgrammeRepository), "Programmes"),
                new RepositoryDetailInfo(typeof(IRatingsScheduleRepository), typeof(FileRatingsScheduleRepository), "RatingsSchedule"),
                new RepositoryDetailInfo(typeof(IRecommendationRepository), typeof(FileRecommendationRepository), "Recommendations"),
                new RepositoryDetailInfo(typeof(IRestrictionRepository), typeof(FileRestrictionRepository), "Restrictions"),
                new RepositoryDetailInfo(typeof(ISalesAreaRepository), typeof(FileSalesAreaRepository), "SalesAreas"),
                new RepositoryDetailInfo(typeof(IScheduleRepository), typeof(FileScheduleRepository), "Schedules"),
                new RepositoryDetailInfo(typeof(ISmoothConfigurationRepository), typeof(FileSmoothConfigurationRepository), "SmoothConfigurations"),
                new RepositoryDetailInfo(typeof(ISmoothFailureRepository), typeof(FileSmoothFailureRepository), "SmoothFailures"),
                new RepositoryDetailInfo(typeof(ISmoothFailureMessageRepository), typeof(FileSmoothFailureMessageRepository), "SmoothFailureMessages"),
                new RepositoryDetailInfo(typeof(ISponsorshipRepository), typeof(FileSponsorshipRepository), "Sponsorships"),
                new RepositoryDetailInfo(typeof(ISpotPlacementRepository), typeof(FileSpotPlacementRepository), "SpotPlacements"),
                new RepositoryDetailInfo(typeof(ISpotRepository), typeof(FileSpotRepository), "Spots"),
                new RepositoryDetailInfo(typeof(ITenantSettingsRepository), typeof(FileTenantSettingsRepository), "TenantSettings"),
                new RepositoryDetailInfo(typeof(IUniverseRepository), typeof(FileUniverseRepository), "Universes"),
            };

        /// <summary>
        /// Details of a repository
        /// </summary>
        private readonly struct RepositoryDetailInfo
        {
            public Type InterfaceType { get; }

            public Type ImplementationType { get; }

            public string FolderName { get; }

            public RepositoryDetailInfo(Type interfaceType, Type implementationType, string folderName)
            {
                InterfaceType = interfaceType;
                ImplementationType = implementationType;
                FolderName = folderName;
            }
        }

        // Root folder, each repo has a separate folder
        private readonly string _rootFolder;

        public FileRepositoryScope(string rootFolder) => _rootFolder = rootFolder;

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

            foreach (var repositoryType in repositoryTypes)
            {
                RepositoryDetailInfo repositoryDetailsInfo =
                    Array.Find(RepositoryDetails, rdi => Match(rdi, repositoryType));

                object repository = CreateRepository(repositoryDetailsInfo);
                repositories.Add(repositoryType, repository);
            }

            return repositories;

            static bool Match(RepositoryDetailInfo ri, Type repositoryType) =>
                ri.InterfaceType == repositoryType || ri.ImplementationType == repositoryType;
        }

        /// <summary>
        /// Creates a repository of the specified type
        /// </summary>
        /// <param name="repositoryDetails"></param>
        /// <returns></returns>
        private object CreateRepository(RepositoryDetailInfo repositoryDetails) =>
            Activator.CreateInstance(
                repositoryDetails.ImplementationType,
                Path.Combine(_rootFolder, repositoryDetails.FolderName));

        public void Dispose()
        {
            //nothing to dispose for file repository scope implementation
        }
    }
}
