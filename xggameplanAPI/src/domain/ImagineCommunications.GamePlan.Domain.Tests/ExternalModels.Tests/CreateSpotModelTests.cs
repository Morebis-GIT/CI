using AutoFixture;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using NUnit.Framework;
using xggameplan.Model;

namespace ImagineCommunications.GamePlan.Domain.Tests.ExternalModels.Tests
{
    [TestFixture(Category = "Domain (external) models :: " + nameof(CreateSpot))]
    public class CreateSpotModelTests
    {
        private const string ValidBreakType = "VA-ValidBreakType";
        private const string InvalidBreakType_TooShort = "X";

        private readonly Fixture _fixture = new SafeFixture();

        [Test(Description = "BreakType::Null break type is valid")]
        public void NullBreakTypeIsValid()
        {
            // Assert
            Assert.DoesNotThrow(() =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateSpot>()
                    .Without(p => p.BreakType)
                    .Create();

                // Act
                sample.Validate();
            });
        }

        [Test(Description = "BreakType::White space only break type is invalid")]
        public void WhiteSpaceOnlyBreakTypeIsInvalid()
        {
            // Assert
            _ = Assert.Catch<BreakTypeTooShortException>(() =>
             {
                 // Arrange
                 var sample = _fixture
                      .Build<CreateSpot>()
                      .With(p => p.BreakType, "   ")
                      .Create();

                 // Act
                 sample.Validate();
             });
        }

        [Test(Description = "BreakType::Invalid Break type has a length less than two")]
        public void InvalidBreakTypeHasLengthLessThanTwo()
        {
            // Assert
            _ = Assert.Catch<BreakTypeTooShortException>(() =>
              {
                  // Arrange
                  var sampleToTest = _fixture
                        .Build<CreateSpot>()
                        .With(p => p.BreakType, InvalidBreakType_TooShort)
                        .Create();

                  // Act
                  sampleToTest.Validate();
              });
        }

        [Test(Description = "BreakType::Valid Break type has a length greater than or equal to two")]
        public void ValidBreakTypeHasLengthGreaterThanOrEqualToTwo()
        {
            // Assert
            Assert.DoesNotThrow(() =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateSpot>()
                    .With(p => p.BreakType, ValidBreakType)
                    .Create();

                // Act
                sampleToTest.Validate();
            });
        }
    }
}
