using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    public abstract class AdvertiserExclusivityModelBaseValidationShould<TTarget, TModel>
                        : SponsorshipModelValidationTestBase<TTarget, TModel>
    where TTarget : AdvertiserExclusivityModelBaseValidation<TModel>
    where TModel : AdvertiserExclusivityModelBase
    {
        [SetUp]
        public async Task BeforeEach()
        {
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised();
        }

        [TearDown]
        public async Task AfterEach()
        {
            CleanUpTarget();
            CleanUpDependencies();
        }

        protected override void AssumeDependenciesAreSupplied()
        {
            ProductRepository = new Mock<IProductRepository>();
        }

        protected override void CleanUpDependencies()
        {
            ProductRepository = null;
        }

        private void AssumeTargetIsInitialised()
        {
            AssumeTargetIsInitialised(ProductRepository.Object, null);
        }

        [Test(Description = "Fail When AdvertiserIdentifier Is Not Provided")]
        public async Task FailWhenAdvertiserIdentifierIsNotProvided()
        {
            var model = AssumeValidModelIsSupplied();
            model.AdvertiserIdentifier = null;

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.AdvertiserIdentifier, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Fail When AdvertiserIdentifier Provided Is Empty")]
        public async Task FailWhenAdvertiserIdentifierProvidedIsEmpty()
        {
            var model = AssumeValidModelIsSupplied();
            model.AdvertiserIdentifier = string.Empty;

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.AdvertiserIdentifier, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Fail When AdvertiserIdentifier Provided Do Not Exists")]
        public async Task FailWhenAdvertiserIdentifierProvidedDoNotExists()
        {
            var model = AssumeValidModelIsSupplied();
            AssumeProductRepositoryExistsReturns(false);

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.AdvertiserIdentifier, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Pass When AdvertiserIdentifier Provided Exists")]
        public async Task PassWhenAdvertiserIdentifierProvidedExists()
        {
            var model = AssumeValidModelIsSupplied();
            AssumeProductRepositoryExistsReturns(true);

            Target.ShouldNotHaveValidationErrorFor(a => a.AdvertiserIdentifier, model);
        }

        private void AssumeProductRepositoryExistsReturns(bool valueToReturn)
        {
            _ = ProductRepository.Setup(a => a.Exists(It.IsAny<Expression<Func<Product, bool>>>())).Returns(valueToReturn);
        }
    }
}
