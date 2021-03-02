using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    public abstract class ClashExclusivityModelBaseValidationShould<TTarget, TModel>
                        : SponsorshipModelValidationTestBase<TTarget, TModel>
    where TTarget : ClashExclusivityModelBaseValidation<TModel>
    where TModel : ClashExclusivityModelBase
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
            ClashRepository = new Mock<IClashRepository>();
        }

        protected override void CleanUpDependencies()
        {
            ClashRepository = null;
        }

        private void AssumeTargetIsInitialised()
        {
            AssumeTargetIsInitialised(ClashRepository.Object, null);
        }

        [Test(Description = "Fail When ClashExternalRef Is Not Provided")]
        public async Task FailWhenClashExternalRefIsNotProvided()
        {
            var model = AssumeValidModelIsSupplied();
            model.ClashExternalRef = null;

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.ClashExternalRef, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Fail When ClashExternalRef Provided Is Empty")]
        public async Task FailWhenClashExternalRefProvidedIsEmpty()
        {
            var model = AssumeValidModelIsSupplied();
            model.ClashExternalRef = " ";

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.ClashExternalRef, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }
    }
}
