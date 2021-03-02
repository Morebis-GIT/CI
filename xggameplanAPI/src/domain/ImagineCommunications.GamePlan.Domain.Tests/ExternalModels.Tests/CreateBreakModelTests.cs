using System.IO;
using AutoFixture;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using NUnit.Framework;
using xggameplan.Model;

namespace ImagineCommunications.GamePlan.Domain.Tests.ExternalModels.Tests
{
    [TestFixture(Category = "Domain (external) models :: " + nameof(CreateBreak))]
    public class CreateBreakModelTests
    {
        private const string ValidBreakType = "VA-ValidBreakType";
        private const string InvalidBreakType_TooShort = "X";
        private const string InvalidBreakType_WhiteSpaceSecondCharacter = "X - Invalid Because of 2nd character";

        private readonly Fixture _fixture = new SafeFixture();

        [Test(Description = "BreakType::Null break type is invalid")]
        public void NullBreakTypeIsInvalid()
        {
            // Assert
            _ = Assert.Catch<BreakTypeNullException>(() =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateBreak>()
                    .Without(p => p.BreakType)
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sample.Validate();
            });
        }

        [Test(Description = "BreakType::White space only break type is invalid")]
        public void WhiteSpaceOnlyBreakTypeIsInvalid()
        {
            // Assert
            _ = Assert.Catch<BreakTypeNullException>(() =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, "   ")
                    .Without(p => p.ClockHour)
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
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, InvalidBreakType_TooShort)
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sampleToTest.Validate();
            });
        }

        [Test(Description = "BreakType::Invalid Break type has a length greater than or equal to two but the first")]
        public void InvalidBreakTypeHasCorrectLengthButWhiteSpaceSecondCharacter()
        {
            // Assert
            _ = Assert.Catch<BreakTypePrefixTooShortException>(() =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, InvalidBreakType_WhiteSpaceSecondCharacter)
                    .Without(p => p.ClockHour)
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
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sampleToTest.Validate();
            });
        }

        [Test(Description = "ClockHour::Must be between 0 and 99")]
        public void ClockHourMustBeBetweenZeroAnd99([Range(0, 99)] int value)
        {
            // Assert
            Assert.DoesNotThrow(() =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .With(p => p.ClockHour, value)
                    .Create();

                // Act
                sampleToTest.Validate();
            });
        }

        [Test(Description = "ClockHour::Cannot be less than zero")]
        public void ClockHourCannotBeLessThanZero()
        {
            // Assert
            _ = Assert.Catch<InvalidDataException>(() =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .With(p => p.ClockHour, -1)
                    .Create();

                // Act
                sampleToTest.Validate();
            });
        }

        [Test(Description = "ClockHour::Cannot be greater than 99")]
        public void ClockHourCannotBeGreaterThan99()
        {
            // Assert
            _ = Assert.Catch<InvalidDataException>(() =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .With(p => p.ClockHour, 100)
                    .Create();

                // Act
                sampleToTest.Validate();
            });
        }
    }
}
