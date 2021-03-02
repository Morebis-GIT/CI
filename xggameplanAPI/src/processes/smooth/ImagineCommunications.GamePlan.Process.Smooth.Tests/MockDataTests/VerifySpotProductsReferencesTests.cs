using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Trait("Smooth", "LogIssuesWithSpotProducts")]
    public class VerifySpotProductsReferencesTests
    {
        private readonly Fixture _fixture;
        private readonly string _dummySalesAreaName;

        public VerifySpotProductsReferencesTests()
        {
            // I take my definitions from here: https://martinfowler.com/articles/mocksArentStubs.html
            _fixture = new Fixture();
            _dummySalesAreaName = _fixture.Create<string>();
        }

        [Fact]
        public void WhenSpotReferencedProductIsNotFoundThenLogWarning()
        {
            // Arrange
            var spot = _fixture
                .Build<Spot>()
                .With(p => p.Product, _fixture.Create<string>())
                .Create();

            IImmutableDictionary<string, Product> productsByExternalReferences = Enumerable.Empty<Product>().IndexListByExternalID();
            var clashesByExternalReferences = ImmutableDictionary<string, Clash>.Empty;
            var logWarningMessages = new List<string>();

            // Act
            IReadOnlyCollection<Spot> allSpots = new List<Spot> { spot };

            VerifyModel.VerifySpotProductsReferences(
                _dummySalesAreaName,
                allSpots,
                productsByExternalReferences,
                clashesByExternalReferences,
                logWarningMessages.Add
                );

            // Assert
            Assert.Contains(
                $"Product {spot.Product} for sales area {_dummySalesAreaName} is referenced by " +
                $"spot(s) {spot.ExternalSpotRef} but it does not exist",
                logWarningMessages
                );
        }

        [Fact]
        public void WhenSpotProductReferencedClashIsNotFoundThenLogWarning()
        {
            // Arrange
            var product = _fixture
                .Build<Product>()
                .With(p => p.ClashCode, _fixture.Create<string>())
                .Create();

            var spot = _fixture
                .Build<Spot>()
                .With(p => p.Product, product.Externalidentifier)
                .Create();

            var productsByExternalReferences = new[] { product }.IndexListByExternalID();

            var clashesByExternalReferences = ImmutableDictionary<string, Clash>.Empty;
            var logWarningMessages = new List<string>();

            // Act
            IReadOnlyCollection<Spot> allSpots = new List<Spot> { spot };

            VerifyModel.VerifySpotProductsReferences(
                _dummySalesAreaName,
                allSpots,
                productsByExternalReferences,
                clashesByExternalReferences,
                logWarningMessages.Add
                );

            // Assert
            Assert.Contains(
                $"Product {product.Externalidentifier} for sales area {_dummySalesAreaName} references " +
                $"clash code {product.ClashCode} but the clash does not exist",
                logWarningMessages
                );
        }

        [Fact]
        public void WhenSpotProductDoesNotHaveClashThenThatIsValidAndDoNotLogWarning()
        {
            // Arrange
            var product = _fixture
                .Build<Product>()
                .Without(p => p.ClashCode)
                .Create();

            var spot = _fixture
                .Build<Spot>()
                .With(p => p.Product, product.Externalidentifier)
                .Create();

            var productsByExternalReferences = new[] { product }.IndexListByExternalID();

            var clashesByExternalReferences = ImmutableDictionary<string, Clash>.Empty;
            var logWarningMessages = new List<string>();

            // Act
            IReadOnlyCollection<Spot> allSpots = new List<Spot> { spot };

            VerifyModel.VerifySpotProductsReferences(
                _dummySalesAreaName,
                allSpots,
                productsByExternalReferences,
                clashesByExternalReferences,
                logWarningMessages.Add
                );

            // Assert
            Assert.Empty(logWarningMessages);
        }

        [Fact]
        public void WhenSpotProductClashParentIsNotFoundThenLogWarning()
        {
            // Arrange
            var clash = _fixture
                .Build<Clash>()
                .With(p => p.ParentExternalidentifier, _fixture.Create<string>())
                .Create();

            var product = _fixture
                .Build<Product>()
                .With(p => p.ClashCode, clash.Externalref)
                .Create();

            var spot = _fixture
                .Build<Spot>()
                .With(p => p.Product, product.Externalidentifier)
                .Create();

            var productsByExternalReferences = new[] { product }.IndexListByExternalID();

            var clashesByExternalReferences = Clash.IndexListByExternalRef(
                new List<Clash> { clash }
                );

            var logWarningMessages = new List<string>();

            // Act
            IReadOnlyCollection<Spot> allSpots = new List<Spot> { spot };

            VerifyModel.VerifySpotProductsReferences(
                _dummySalesAreaName,
                allSpots,
                productsByExternalReferences,
                clashesByExternalReferences,
                logWarningMessages.Add
                );

            // Assert
            Assert.Contains(
                $"Product {product.Externalidentifier} for sales area {_dummySalesAreaName} " +
                $"references clash code {product.ClashCode} but the parent clash " +
                $"{clash.ParentExternalidentifier} does not exist",
                logWarningMessages
                );
        }

        [Fact]
        public void WhenSpotProductClashDoesNotHaveParentThenIsValidAndDoNotLogWarning()
        {
            // Arrange
            var clash = _fixture
                .Build<Clash>()
                .With(p => p.ParentExternalidentifier, (string)null)
                .Create();

            var product = _fixture
                .Build<Product>()
                .With(p => p.ClashCode, clash.Externalref)
                .Create();

            var spot = _fixture
                .Build<Spot>()
                .With(p => p.Product, product.Externalidentifier)
                .Create();

            var productsByExternalReferences = new[] { product }.IndexListByExternalID();

            var clashesByExternalReferences = Clash.IndexListByExternalRef(
                new List<Clash> { clash }
                );

            var logWarningMessages = new List<string>();

            // Act
            IReadOnlyCollection<Spot> allSpots = new List<Spot> { spot };

            VerifyModel.VerifySpotProductsReferences(
                _dummySalesAreaName,
                allSpots,
                productsByExternalReferences,
                clashesByExternalReferences,
                logWarningMessages.Add
                );

            // Assert
            Assert.Empty(logWarningMessages);
        }
    }
}
