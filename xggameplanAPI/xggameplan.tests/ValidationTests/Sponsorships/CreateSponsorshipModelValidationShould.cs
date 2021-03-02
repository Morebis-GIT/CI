using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: CreateSponsorshipModelValidationShould")]
    public class CreateSponsorshipModelValidationShould
               : SponsorshipModelValidationTestBase<CreateSponsorshipModelValidation, CreateSponsorshipModel>
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

        [Test(Description = "Fail When ExternalReferenceId Supplied Is Empty")]
        public async Task FailWhenExternalReferenceIdSuppliedIsEmpty()
        {
            var model = AssumeValidModelIsSupplied();
            model.ExternalReferenceId = string.Empty;

            var validationResult = Target.Validate(model, a => a.ExternalReferenceId);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When ExternalReferenceId Supplied Is Null")]
        public async Task FailWhenExternalReferenceIdSuppliedIsNull()
        {
            var model = AssumeValidModelIsSupplied();
            model.ExternalReferenceId = null;

            var validationResult = Target.Validate(model, a => a.ExternalReferenceId);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When ExternalReferenceId Supplied Contains Only WhiteSpace")]
        public async Task FailWhenExternalReferenceIdSuppliedContainsOnlyWhiteSpace()
        {
            var model = AssumeValidModelIsSupplied();
            model.ExternalReferenceId = "  ";

            var validationResult = Target.Validate(model, a => a.ExternalReferenceId);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When ExternalReferenceId Supplied Is Not Unique")]
        public async Task FailWhenExternalReferenceIdSuppliedIsNotUnique()
        {
            var model = AssumeValidModelIsSupplied();
            AssumeSponsorshipRepositoryExistsIsSetupToReturn(true);

            var validationResult = Target.Validate(model, a => a.ExternalReferenceId);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Pass When ExternalReferenceId Supplied Is Unique")]
        public async Task PassWhenExternalReferenceIdSuppliedIsUnique()
        {
            var model = AssumeValidModelIsSupplied();
            AssumeSponsorshipRepositoryExistsIsSetupToReturn(false);

            var validationResult = Target.Validate(model, a => a.ExternalReferenceId);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Fail When No SponsoredItems Is Supplied")]
        public async Task FailWhenNoSponsoredItemsIsSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.SponsoredItems = null;

            var validationResult = Target.Validate(model, a => a.SponsoredItems);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When SponsoredItems Supplied Is Empty")]
        public async Task FailWhenSponsoredItemsSuppliedIsEmpty()
        {
            var model = AssumeValidModelIsSupplied();
            model.SponsoredItems = new List<CreateSponsoredItemModel>();

            var validationResult = Target.Validate(model, a => a.SponsoredItems);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        protected override void AssumeDependenciesAreSupplied()
        {
            SponsorshipRepository = new Mock<ISponsorshipRepository>();
        }

        protected override void CleanUpDependencies()
        {
            SponsorshipRepository = null;
        }

        private void AssumeTargetIsInitialised()
        {
            AssumeTargetIsInitialised(SponsorshipRepository.Object, null, null, null, null, null);
        }

        private void AssumeSponsorshipRepositoryExistsIsSetupToReturn(bool valueToReturn)
        {
            _ = SponsorshipRepository.Setup(s => s.Exists(It.IsAny<string>())).Returns(valueToReturn);
        }
    }
}
