using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.CommonTestsHelpers
{
    public class RepositoryWrapper
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public RepositoryWrapper(IRepositoryFactory repositoryFactory) =>
            _repositoryFactory = repositoryFactory;

        public Break LoadTestBreak(Guid breakId)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var breakRepository = scope.CreateRepository<IBreakRepository>();

            return breakRepository.Get(breakId);
        }

        public Spot LoadTestSpot(Guid spotId)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var spotRepository = scope.CreateRepository<ISpotRepository>();

            return spotRepository.Find(spotId);
        }

        public IEnumerable<Break> LoadAllTestBreaks()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var breakRepository = scope.CreateRepository<IBreakRepository>();

            return breakRepository.GetAll();
        }

        public IEnumerable<Clash> LoadAllClashes()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var clashRepository = scope.CreateRepository<IClashRepository>();

            return clashRepository.GetAll();
        }

        public IEnumerable<ClashException> LoadAllClashExceptions()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var clashExceptionRepository = scope.CreateRepository<IClashExceptionRepository>();

            return clashExceptionRepository.GetAll();
        }

        public IEnumerable<Product> LoadAllProducts()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var productRepository = scope.CreateRepository<IProductRepository>();

            return productRepository.GetAll();
        }

        public IEnumerable<Programme> LoadAllProgrammes()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var programmeRepository = scope.CreateRepository<IProgrammeRepository>();

            return programmeRepository.GetAll();
        }

        public IEnumerable<Spot> LoadAllSpots()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var spotRepository = scope.CreateRepository<ISpotRepository>();

            return spotRepository.GetAll();
        }

        public IEnumerable<SalesArea> LoadAllSalesArea()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();

            return salesAreaRepository.GetAll();
        }

        public void SaveTestSample<TType>(TType sample)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();

            switch (sample)
            {
                case Break sampleBreak:
                    SaveTestSample(scope, sampleBreak);
                    break;

                case Spot sampleSpot:
                    SaveTestSample(scope, sampleSpot);
                    break;
            }
        }

        private void SaveTestSample(IRepositoryScope scope, Break aBreak)
        {
            var repository = scope.CreateRepository<IBreakRepository>();
            repository.Add(aBreak);
            repository.SaveChanges();
        }

        private void SaveTestSample(IRepositoryScope scope, Spot aSpot)
        {
            var repository = scope.CreateRepository<ISpotRepository>();

            repository.Add(aSpot);
            repository.SaveChanges();
        }
    }
}
