using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class ModelLoaders
        : IModelLoaders
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ModelLoaders(IRepositoryFactory repositoryFactory) =>
            _repositoryFactory = repositoryFactory;

        private static TRepository CreateRepository<TRepository>(IRepositoryScope scope)
            where TRepository : class =>
            scope.CreateRepository<TRepository>();

        private IImmutableList<TModel> GetAllFrom<TRepository, TModel>()
            where TRepository : class, IRepository<TModel>
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<TRepository>(scope);

            return repo.GetAll().ToImmutableList();
        }

        public IImmutableList<Clash> GetAllClashes() =>
            GetAllFrom<IClashRepository, Clash>();

        public IImmutableList<ClashException> GetAllClashException() =>
            GetAllFrom<IClashExceptionRepository, ClashException>();

        public IImmutableList<Product> GetAllProducts() =>
            GetAllFrom<IProductRepository, Product>();

        public IImmutableList<Campaign> GetAllCampaigns()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<ICampaignRepository>(scope);

            return repo.GetAll().ToImmutableList();
        }

        public IImmutableList<Restriction> GetAllRestrictions()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<IRestrictionRepository>(scope);

            return repo.GetAll().ToImmutableList();
        }

        public IImmutableList<IndexType> GetAllIndexTypes()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<IIndexTypeRepository>(scope);

            return repo.GetAll().ToImmutableList();
        }

        public IImmutableList<Universe> GetAllUniverses()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<IUniverseRepository>(scope);
            return repo.GetAll().ToImmutableList();
        }

        public IImmutableList<SmoothFailureMessage> GetAllSmoothFailureMessages()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<ISmoothFailureMessageRepository>(scope);

            return repo
                .GetAll()
                .OrderBy(sf => sf.Id)
                .ToImmutableList();
        }

        public IImmutableList<Break> GetAllBreaksForSalesAreaForSmoothPeriod(
            DateTimeRange smoothPeriod,
            IReadOnlyCollection<string> salesAreaNames
            )
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<IBreakRepository>(scope);

            return repo
                .Search(smoothPeriod, salesAreaNames)
                .ToImmutableList();
        }

        public SmoothConfiguration GetSmoothConfiguration()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<ISmoothConfigurationRepository>(scope);

            return repo.GetById(SmoothConfiguration.DefaultId);
        }

        public IImmutableList<Sponsorship> GetAllSponsorshipRestrictions()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = CreateRepository<ISponsorshipRepository>(scope);

            var result = repo.GetAll();

            if (!result.Any())
            {
                result = ImmutableList<Sponsorship>.Empty;
            }

            return result.ToImmutableList();
        }
    }
}
