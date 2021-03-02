using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using Moq;
using NUnit.Framework;
using xggameplan.core.tests.Validators.TestData;
using xggameplan.core.Validations;

namespace xggameplan.core.tests.Validators
{
    [TestFixture(Category = "Validations :: ClashExceptions")]
    public class ClashExceptionValidationTest : IDisposable
    {
        private IClashExceptionValidations _clashExceptionValidations;
        private Mock<IClashRepository> _fakeClashRepository;

        [SetUp]
        public void Init()
        {
            _fakeClashRepository = new Mock<IClashRepository>();
            _clashExceptionValidations = new ClashExceptionValidations(_fakeClashRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _fakeClashRepository = null;
            _clashExceptionValidations = null;
        }

        [TestCaseSource(typeof(ClashExceptionValidationTestData),
            nameof(ClashExceptionValidationTestData.ClashExceptionsOverlapValidationData))]
        public void ValidateClashExceptionOverlapsWithExistingClashExceptions(
            IEnumerable<ClashException> givenExceptions,
            int offsetHours,
            IEnumerable<ClashException> existingExceptions,
            CustomValidationResult expectedResult
        )
        {
            var result =
                _clashExceptionValidations.ValidateTimeRanges(givenExceptions, offsetHours, existingExceptions);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.Successful, expectedResult.Successful);
                Assert.AreEqual(result.Message, expectedResult.Message);
            });
        }

        [TestCaseSource(typeof(ClashExceptionValidationTestData),
            nameof(ClashExceptionValidationTestData.ClashExceptionSameStructureValidation))]
        public void ValidateClashExceptionSameStructureRulesViolation(
            ClashException clashException,
            CustomValidationResult expectedResult
        )
        {
            _ = _fakeClashRepository
                    .Setup(r => r.FindByExternal(It.IsAny<string>()))
                    .Returns<string>(externalIdentifier =>
                        ClashExceptionValidationTestData.BunchOfClashes.Where(c => c.Externalref == externalIdentifier)
                        );

            var result =
                _clashExceptionValidations.ValidateClashExceptionForSameStructureRulesViolation(clashException);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.Successful, expectedResult.Successful);
                Assert.AreEqual(result.Message, expectedResult.Message);
            });
        }

        public void Dispose()
        {
            _clashExceptionValidations = null;
            _fakeClashRepository = null;
        }
    }
}
