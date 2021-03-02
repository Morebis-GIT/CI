using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using xggameplan.Model;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.ExternalModels.Tests
{
    [Trait("Domain (external) models", nameof(CreateBreak))]
    public class CreateBreakModelTests
    {
        private const string ValidBreakType = "VA-ValidBreakType";
        private const string InvalidBreakType_TooShort = "X";
        private const string InvalidBreakType_WhiteSpaceSecondCharacter = "X - Invalid Because of 2nd character";

        private readonly Fixture _fixture = new SafeFixture();

        [Fact(DisplayName = "BreakType::Null break type is invalid")]
        public void NullBreakTypeIsInvalid()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateBreak>()
                    .Without(p => p.BreakType)
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sample.Validate();
            };

            _ = act.Should().Throw<BreakTypeNullException>();
        }

        [Fact(DisplayName = "BreakType::White space only break type is invalid")]
        public void WhiteSpaceOnlyBreakTypeIsInvalid()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, "   ")
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sample.Validate();
            };

            _ = act.Should().Throw<BreakTypeNullException>();
        }

        [Fact(DisplayName = "BreakType::Invalid Break type has a length less than two")]
        public void InvalidBreakTypeHasLengthLessThanTwo()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, InvalidBreakType_TooShort)
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sampleToTest.Validate();
            };

            _ = act.Should().Throw<BreakTypeTooShortException>();
        }

        [Fact(DisplayName = "BreakType::Invalid Break type has a length greater than or equal to two but the first")]
        public void InvalidBreakTypeHasCorrectLengthButWhiteSpaceSecondCharacter()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, InvalidBreakType_WhiteSpaceSecondCharacter)
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sampleToTest.Validate();
            };

            _ = act.Should().Throw<BreakTypePrefixTooShortException>();
        }

        [Fact(DisplayName = "BreakType::Valid Break type has a length greater than or equal to two")]
        public void ValidBreakTypeHasLengthGreaterThanOrEqualToTwo()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .Without(p => p.ClockHour)
                    .Create();

                // Act
                sampleToTest.Validate();
            };

            act.Should().NotThrow();
        }

        [Theory(DisplayName = "ClockHour::Must be between 0 and 99")]
        [MemberData(nameof(ClockHourRange))]
        public void ClockHourMustBeBetweenZeroAnd99(int value)
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .With(p => p.ClockHour, value)
                    .Create();

                // Act
                sampleToTest.Validate();
            };

            act.Should().NotThrow();
        }

        public static IEnumerable<object[]> ClockHourRange()
        {
            for (int i = 0; i <= 99; i++)
            {
                yield return new object[] { i };
            }
        }

        [Fact(DisplayName = "ClockHour::Cannot be less than zero")]
        public void ClockHourCannotBeLessThanZero()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .With(p => p.ClockHour, -1)
                    .Create();

                // Act
                sampleToTest.Validate();
            };

            _ = act.Should().Throw<InvalidDataException>();
        }

        [Fact(DisplayName = "ClockHour::Cannot be greater than 99")]
        public void ClockHourCannotBeGreaterThan99()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateBreak>()
                    .With(p => p.BreakType, ValidBreakType)
                    .With(p => p.ClockHour, 100)
                    .Create();

                // Act
                sampleToTest.Validate();
            };

            _ = act.Should().Throw<InvalidDataException>();
        }
    }
}
