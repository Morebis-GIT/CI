using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.TypeTests
{
    [Collection("Smooth types")]
    [Trait("Types", "BreakExternalReference")]
    public class BreakExternalReferenceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void AssigningAValue(string value)
        {
            // Arrange

            // Act
            BreakExternalReference result = value;

            // Assert
            _ = value.Should().BeEquivalentTo(result, becauseArgs: null);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void AssigningToAString(string value)
        {
            // Arrange
            BreakExternalReference subject = value;

            // Act
            string result = subject;

            // Assert
            _ = value.Should().BeEquivalentTo(result, becauseArgs: null);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void BreakExternalReferenceToStringEquality(string value)
        {
            // Arrange
            BreakExternalReference subject = value;

            // Act
            bool result = subject.Equals(value);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToBreakExternalReferenceEquality(string value)
        {
            // Arrange
            BreakExternalReference subject = value;

            // Act
            bool result = value.Equals(subject);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToBreakExternalReferenceEqualityOperator(string value)
        {
            // Arrange
            BreakExternalReference subject = value;

            // Act
            bool result = value == subject;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToBreakExternalReferenceInequalityOperator(string value)
        {
            // Arrange
            BreakExternalReference subject = value;

            // Act
            bool result = subject != "Badger";

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void BreakExternalReferenceToBreakExternalReferenceEquality(
            string left,
            string right
            )
        {
            // Arrange
            BreakExternalReference subjectLeft = left;
            BreakExternalReference subjectRight = right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void BreakExternalReferenceToBreakExternalReferenceAsObjectEquality(
            string left,
            string right
            )
        {
            // Arrange
            BreakExternalReference subjectLeft = left;
            object subjectRight = (BreakExternalReference)right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void BreakExternalReferenceToStringAsObjectEquality(
            string left,
            string right
            )
        {
            // Arrange
            BreakExternalReference subjectLeft = left;
            object subjectRight = right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void AssignmentIsNewInstance(string value)
        {
            // Arrange

            // Act
            BreakExternalReference subject = value;

            // Assert
            _ = subject.Should().NotBeSameAs(value, becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void Appending(string value)
        {
            // Arrange
            BreakExternalReference subject = value;
            BreakExternalReference subjectAppended = subject + value;

            // Act
            bool result = subjectAppended == subject + value;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }
    }
}
