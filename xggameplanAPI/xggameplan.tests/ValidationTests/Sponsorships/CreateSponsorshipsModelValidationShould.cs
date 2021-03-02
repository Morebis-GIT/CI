using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: CreateSponsorshipsModelValidationShould")]
    public class CreateSponsorshipsModelValidationShould
             : SponsorshipModelValidationTestBase<CreateSponsorshipsModelValidation, IEnumerable<CreateSponsorshipModel>>
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

        [Test(Description = "Fail When ExternalReferenceIds Supplied Are Not Unique")]
        public async Task FailWhenExternalReferenceIdsSuppliedAreNotUnique()
        {
            var models = CreateValidModels<CreateSponsorshipModel>(3);
            foreach (var item in models)
            {
                item.ExternalReferenceId = "ExternalRef1";
            }

            var validationResult = Target.Validate(models);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When ExternalReferenceIds Supplied Contains a Null ExternalReferenceId")]
        public async Task FailWhenExternalReferenceIdsSuppliedContainsaNullExternalReferenceId()
        {
            var modelWithNullExternalReferenceId = CreateValidModel<CreateSponsorshipModel>();
            modelWithNullExternalReferenceId.ExternalReferenceId = null;
            var models = CreateValidModels<CreateSponsorshipModel>(3).ToList();
            models.Add(modelWithNullExternalReferenceId);

            var validationResult = Target.Validate(models);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When ExternalReferenceIds Supplied Contains An Empty ExternalReferenceId")]
        public async Task FailWhenExternalReferenceIdsSuppliedContainsAnEmptyExternalReferenceId()
        {
            var modelWithEmptyExternalReferenceId = CreateValidModel<CreateSponsorshipModel>();
            modelWithEmptyExternalReferenceId.ExternalReferenceId = string.Empty;
            var models = CreateValidModels<CreateSponsorshipModel>(3).ToList();
            models.Add(modelWithEmptyExternalReferenceId);

            var validationResult = Target.Validate(models);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        private void AssumeTargetIsInitialised()
        {
            AssumeTargetIsInitialised(SponsorshipRepository.Object, null, null, null, null, null);
        }
    }
}
