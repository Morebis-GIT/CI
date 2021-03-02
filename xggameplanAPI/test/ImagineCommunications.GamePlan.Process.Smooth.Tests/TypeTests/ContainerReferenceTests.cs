using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.TypeTests
{
    [Collection("Smooth types")]
    [Trait("Types", nameof(ContainerReference))]
    public class ContainerReferenceTests
    {
        [Theory(DisplayName =
            "Given an invalid container reference value " +
            "When assigning the value to a ContainerReference " +
            "Then an exception is thrown.")]
        [InlineData(null)]
        [InlineData("Hi")]
        [InlineData("break-part2-3")]
        [InlineData("break-3")]
        [InlineData("break-1-SALESAREA-TODAYDATE")]
        [InlineData("break-part1-SALESAREA-TODAYDATE")]
        public void InvalidContainerReferenceValueThrows(string value)
        {
            // Arrange

            // Act
            Action act = () => { ContainerReference labrat = value; };

            // Assert
            _ = act.Should().Throw<ArgumentException>(null);
        }

        [Theory(DisplayName =
            "Given a valid container reference " +
            "When reading the position of the container parsed from the container reference " +
            "Then the value should be the container number.")]
        [InlineData("break-1-2", 1)]
        [InlineData("break-2-2", 2)]
        [InlineData("break-3-2", 3)]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", 1)]
        public void PositionProperty(string value, int expectedValue)
        {
            // Arrange
            ContainerReference labrat = value;

            // Act
            var result = labrat.ContainerNumber;

            // Assert
            _ = result.Should().Be(expectedValue, null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1")]
        [InlineData("break-2-2", "break-2")]
        [InlineData("break-3-2", "break-3")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1")]
        public void AssigningAStringToAContainerReference(string value, string expected)
        {
            // Arrange

            // Act
            ContainerReference result = value;

            // Assert
            _ = result.ToString().Should().BeEquivalentTo(expected, becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1")]
        [InlineData("break-2-2", "break-2")]
        [InlineData("break-3-2", "break-3")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1")]
        public void AssigningContainerReferenceToAString(string value, string expected)
        {
            // Arrange
            ContainerReference subject = value;

            // Act
            string result = subject;

            // Assert
            _ = result.Should().BeEquivalentTo(expected, becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1")]
        [InlineData("break-2-2", "break-2")]
        [InlineData("break-3-2", "break-3")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1")]
        public void ContainerReferenceToString_EqualityMethod(string value, string expected)
        {
            // Arrange
            ContainerReference subject = value;

            // Act
            bool result = subject.Equals(expected);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1")]
        [InlineData("break-2-2", "break-2")]
        [InlineData("break-3-2", "break-3")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1")]
        public void ContainerReferenceToString_EqualityOperator(string value, string expected)
        {
            // Arrange
            ContainerReference subject = value;

            // Act
            bool result = subject == expected;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1")]
        [InlineData("break-2-2", "break-2")]
        [InlineData("break-3-2", "break-3")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1")]
        public void StringToContainerReference_EqualityMethod(string value, string expected)
        {
            // Arrange
            ContainerReference subject = value;

            // Act
            bool result = expected.Equals(subject);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2")]
        [InlineData("break-2-2")]
        [InlineData("break-3-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE")]
        public void StringToContainerReference_EqualityOperator(string value)
        {
            // Arrange
            ContainerReference subject = value;

            // Act
            bool result = value == subject;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2")]
        [InlineData("break-2-2")]
        [InlineData("break-3-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE")]
        public void StringToContainerReference_InequalityOperator(string value)
        {
            // Arrange
            ContainerReference subject = value;

            // Act
            bool result = subject != "break-99-98";

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1-2")]
        [InlineData("break-2-2", "break-2-2")]
        [InlineData("break-3-2", "break-3-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1-2-SALESAREA-TODAYDATE")]
        public void ContainerReferenceToContainerReference_EqualityMethod(
            string left,
            string right
            )
        {
            // Arrange
            ContainerReference subjectLeft = left;
            ContainerReference subjectRight = right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1-2")]
        [InlineData("break-2-2", "break-2-2")]
        [InlineData("break-3-2", "break-3-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1-2-SALESAREA-TODAYDATE")]
        public void ContainerReferenceToContainerReferenceAsObject_EqualityMethod(
            string left,
            string right
            )
        {
            // Arrange
            ContainerReference subjectLeft = left;
            object subjectRight = (ContainerReference)right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2", "break-1")]
        [InlineData("break-2-2", "break-2")]
        [InlineData("break-3-2", "break-3")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE", "break-1")]
        public void ContainerReferenceToStringAsObject_EqualityMethod(
            string left,
            string right
            )
        {
            // Arrange
            ContainerReference subjectLeft = left;
            object subjectRight = right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("break-1-2")]
        [InlineData("break-2-2")]
        [InlineData("break-3-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE")]
        public void AssignmentIsNewInstance(string value)
        {
            // Arrange
            ContainerReference left = value;
            ContainerReference right = value;

            // Act
            /* Empty */

            // Assert
            _ = left.Should().NotBeSameAs(right, becauseArgs: null);
        }

        [Theory(DisplayName =
            "Given a break external reference is not in container format " +
            "When parsing the break external reference against the container policy " +
            "Then the parser should return false.")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("part1")]
        [InlineData("part1-part2")]
        [InlineData("part1-1-part3")]
        [InlineData("part1-part2-3")]
        [InlineData("part1-part2-part3")]
        public void TryParse_BreakExternalReferenceIsNotContainer_ReturnsFalse(string breakExternalReference)
        {
            // Arrange

            // Act
            bool result = ContainerReference.TryParse(breakExternalReference, out ContainerReference _);

            // Assert
            _ = result.Should().BeFalse(null);
        }

        [Theory(DisplayName =
            "Given a break external reference is in container format " +
            "When parsing the break external reference against the container policy " +
            "Then the parser should return true " +
            "And the output object is Container Reference")]
        [InlineData("break-1-2")]
        [InlineData("break-1-2-SALESAREA-TODAYDATE")]
        public void TryParse_BreakExternalReferenceIsContainer_ReturnsTrue(string breakExternalReference)
        {
            // Arrange

            // Act
            bool result = ContainerReference.TryParse(breakExternalReference, out ContainerReference cr);

            // Assert
            _ = result.Should().BeTrue(null);
            _ = cr.ToString().Should().Be("break-1", null);
        }
    }
}
