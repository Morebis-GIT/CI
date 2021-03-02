using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: UpdateSponsorshipsModelValidatorShould")]
    public class UpdateSponsorshipsModelValidatorShould
    {
        private Fixture _fixture;
        private UpdateSponsorshipModelValidator _target;
        private Mock<IValidator<UpdateSponsorshipModel>> _updateSponsorshipsModelValidation;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fixture = new Fixture();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _fixture = null;
        }

        [SetUp]
        public void SetUp()
        {
            AssumeDependencyIsSupplied();
            AssumeTargetIsInitialised();
        }

        [TearDown]
        public void TearDown()
        {
            CleanUpTarget();
            CleanUpDependency();
        }

        [Test(Description = "Contain Errors When UpdateSponsorshipModel Validation Fails")]
        public async Task ContainErrorsWhenCreateSponsorshipModelValidationFails()
        {
            var model = AssumeUpdateSponsorshipModelIsSupplied();
            var validationResultWithErrors = createValidationResultWithErrors(6);
            AssumeUpdateSponsorshipModelValidationValidateReturns(validationResultWithErrors);

            var isValid = _target.IsValid(model);

            using (new AssertionScope())
            {
                _ = isValid.Should().BeFalse();
                _ = _target.Errors.Should().NotBeNullOrEmpty();
                _ = _target.Errors.Count().Should().Be(validationResultWithErrors.Errors.Count);
            }
        }

        [Test(Description = "Fail Validation When UpdateSponsorshipModel Validation Fails")]
        public async Task FailValidationWhenCreateSponsorshipModelValidationFails()
        {
            var model = AssumeUpdateSponsorshipModelIsSupplied();
            var valiationResultWithErrors = createValidationResultWithErrors(2);
            AssumeUpdateSponsorshipModelValidationValidateReturns(valiationResultWithErrors);

            var isValid = _target.IsValid(model);

            _ = isValid.Should().BeFalse();
        }

        [Test(Description = "Pass Validation When UpdateSponsorshipModel Validation Passes")]
        public async Task PassValidationWhenCreateSponsorshipModelValidationPasses()
        {
            var model = AssumeUpdateSponsorshipModelIsSupplied();
            var valiationResultWithNoErrors = createValidationResultWithNoErrors();
            AssumeUpdateSponsorshipModelValidationValidateReturns(valiationResultWithNoErrors);

            var isValid = _target.IsValid(model);

            _ = isValid.Should().BeTrue();
        }

        private void AssumeUpdateSponsorshipModelValidationValidateReturns(ValidationResult validationResult)
        {
            _ = _updateSponsorshipsModelValidation.Setup(a => a.Validate(It.IsAny<UpdateSponsorshipModel>()))
                                              .Returns(validationResult);
        }

        private void AssumeTargetIsInitialised()
        {
            _target = new UpdateSponsorshipModelValidator(_updateSponsorshipsModelValidation.Object);
        }

        private void AssumeDependencyIsSupplied()
        {
            _updateSponsorshipsModelValidation = new Mock<IValidator<UpdateSponsorshipModel>>();
        }

        private UpdateSponsorshipModel AssumeUpdateSponsorshipModelIsSupplied()
        {
            return _fixture.Create<UpdateSponsorshipModel>();
        }

        private ValidationResult createValidationResultWithErrors(int noOfFailures)
        {
            return new ValidationResult(_fixture.CreateMany<ValidationFailure>(noOfFailures));
        }

        private ValidationResult createValidationResultWithNoErrors()
        {
            return createValidationResultWithErrors(0);
        }

        private void CleanUpTarget()
        {
            _target = null;
        }

        private void CleanUpDependency()
        {
            _updateSponsorshipsModelValidation = null;
        }
    }
}
