using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: SponsorshipModelBaseValidationShould")]
    public class SponsorshipModelBaseValidationShould
               : SponsorshipModelValidationTestBase<SponsorshipModelBaseValidation<SponsorshipModelBase>,
                                                    SponsorshipModelBase>
    {
        [SetUp]
        public async Task BeforeEach()
        {
            AssumeTargetIsInitialised();
        }

        [TearDown]
        public async Task AfterEach()
        {
            CleanUpTarget();
        }

        [Test(Description = "Fail When ExternalReferenceId Is Not Supplied")]
        public async Task FailWhenExternalReferenceIdIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.ExternalReferenceId = string.Empty;

            var validationResult = Target.Validate(model, a => a.ExternalReferenceId);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Pass When ExternalReferenceId Is Supplied")]
        public async Task PassWhenExternalReferenceIdIsSupplied()
        {
            var model = AssumeValidModelIsSupplied();

            var validationResult = Target.Validate(model, a => a.ExternalReferenceId);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().Be(0, becauseArgs: null);
            }
        }
    }
}
