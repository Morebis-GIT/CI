using System;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using xggameplan.Model;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.ExternalModels.Tests
{
    [Trait("Domain (external) models", nameof(CreateSpot))]
    public class CreateSpotModelTests
    {
        private const string ValidBreakType = "VA-ValidBreakType";
        private const string InvalidBreakType_TooShort = "X";

        private readonly Fixture _fixture = new SafeFixture();

        [Fact(DisplayName = "BreakType::Null break type is valid")]
        public void NullBreakTypeIsValid()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateSpot>()
                    .Without(p => p.BreakType)
                    .Create();

                // Act
                sample.Validate();
            };

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "BreakType::White space only break type is invalid")]
        public void WhiteSpaceOnlyBreakTypeIsInvalid()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sample = _fixture
                    .Build<CreateSpot>()
                    .With(p => p.BreakType, "   ")
                    .Create();

                // Act
                sample.Validate();
            };

            _ = act.Should().Throw<BreakTypeTooShortException>();
        }

        [Fact(DisplayName = "BreakType::Invalid Break type has a length less than two")]
        public void InvalidBreakTypeHasLengthLessThanTwo()
        {
            // Assert
            Action act = () =>
             {
                 // Arrange
                 var sampleToTest = _fixture
                      .Build<CreateSpot>()
                      .With(p => p.BreakType, InvalidBreakType_TooShort)
                      .Create();

                 // Act
                 sampleToTest.Validate();
             };

            _ = act.Should().Throw<BreakTypeTooShortException>();
        }

        [Fact(DisplayName = "BreakType::Valid Break type has a length greater than or equal to two")]
        public void ValidBreakTypeHasLengthGreaterThanOrEqualToTwo()
        {
            // Assert
            Action act = () =>
            {
                // Arrange
                var sampleToTest = _fixture
                    .Build<CreateSpot>()
                    .With(p => p.BreakType, ValidBreakType)
                    .Create();

                // Act
                sampleToTest.Validate();
            };

            act.Should().NotThrow();
        }
    }
}
