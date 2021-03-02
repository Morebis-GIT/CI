using System.Collections.Generic;
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
    [TestFixture(Category = "Validations :: Sponsorship :: CreateSponsorshipsModelValidatorShould")]
    public class CreateSponsorshipsModelValidatorShould
    {
        private Fixture _fixture;
        private CreateSponsorshipsModelValidator _target;
        private Mock<IValidator<IEnumerable<CreateSponsorshipModel>>> _createSponsorshipsModelValidation;

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

        [Test(Description = "Contain Errors When CreateSponsorshipModel Validation Fails")]
        public async Task ContainErrorsWhenCreateSponsorshipModelValidationFails()
        {
            var model = AssumeCreateSponsorshipModelIsSupplied();
            var validationResultWithErrors = createValidationResultWithErrors(6);
            AssumeCreateSponsorshipModelValidationValidateReturns(validationResultWithErrors);

            var isValid = _target.IsValid(model);

            using (new AssertionScope())
            {
                _ = isValid.Should().BeFalse();
                _ = _target.Errors.Should().NotBeNullOrEmpty();
                _ = _target.Errors.Count().Should().Be(validationResultWithErrors.Errors.Count);
            }
        }

        [Test(Description = "Fail Validation When CreateSponsorshipModel Validation Fails")]
        public async Task FailValidationWhenCreateSponsorshipModelValidationFails()
        {
            var model = AssumeCreateSponsorshipModelIsSupplied();
            var valiationResultWithErrors = createValidationResultWithErrors(2);
            AssumeCreateSponsorshipModelValidationValidateReturns(valiationResultWithErrors);

            var isValid = _target.IsValid(model);

            _ = isValid.Should().BeFalse();
        }

        [Test(Description = "Pass Validation When CreateSponsorshipModel Validation Passes")]
        public async Task PassValidationWhenCreateSponsorshipModelValidationPasses()
        {
            var model = AssumeCreateSponsorshipModelIsSupplied();
            var valiationResultWithNoErrors = createValidationResultWithNoErrors();
            AssumeCreateSponsorshipModelValidationValidateReturns(valiationResultWithNoErrors);

            var isValid = _target.IsValid(model);

            _ = isValid.Should().BeTrue();
        }

        private void AssumeCreateSponsorshipModelValidationValidateReturns(ValidationResult validationResult)
        {
            _ = _createSponsorshipsModelValidation.Setup(a => a.Validate(It.IsAny<IEnumerable<CreateSponsorshipModel>>()))
                                              .Returns(validationResult);
        }

        private void AssumeTargetIsInitialised()
        {
            _target = new CreateSponsorshipsModelValidator(_createSponsorshipsModelValidation.Object);
        }

        private void AssumeDependencyIsSupplied()
        {
            _createSponsorshipsModelValidation = new Mock<IValidator<IEnumerable<CreateSponsorshipModel>>>();
        }

        private IEnumerable<CreateSponsorshipModel> AssumeCreateSponsorshipModelIsSupplied()
        {
            return _fixture.CreateMany<CreateSponsorshipModel>(2);
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
            _createSponsorshipsModelValidation = null;
        }
    }
}
