using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.TypeTests
{
    [Collection("Smooth types")]
    [Trait("Types", "SpotExternalReference")]
    public class SpotExternalReferenceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void AssigningAValue(string value)
        {
            // Arrange

            // Act
            SpotExternalReference result = value;

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
            SpotExternalReference subject = value;

            // Act
            string result = subject;

            // Assert
            _ = value.Should().BeEquivalentTo(result, becauseArgs: null);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void SpotExternalReferenceToStringEquality(string value)
        {
            // Arrange
            SpotExternalReference subject = value;

            // Act
            bool result = subject.Equals(value);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToSpotExternalReferenceEquality(string value)
        {
            // Arrange
            SpotExternalReference subject = value;

            // Act
            bool result = value.Equals(subject);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToSpotExternalReferenceEqualityOperator(string value)
        {
            // Arrange
            SpotExternalReference subject = value;

            // Act
            bool result = value == subject;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToSpotExternalReferenceInequalityOperator(string value)
        {
            // Arrange
            SpotExternalReference subject = value;

            // Act
            bool result = subject != "Badger";

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void SpotExternalReferenceToSpotExternalReferenceEquality(
            string left,
            string right
            )
        {
            // Arrange
            SpotExternalReference subjectLeft = left;
            SpotExternalReference subjectRight = right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void SpotExternalReferenceToSpotExternalReferenceAsObjectEquality(
            string left,
            string right
            )
        {
            // Arrange
            SpotExternalReference subjectLeft = left;
            object subjectRight = (SpotExternalReference)right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void SpotExternalReferenceToStringAsObjectEquality(
            string left,
            string right
            )
        {
            // Arrange
            SpotExternalReference subjectLeft = left;
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
            SpotExternalReference subject = value;

            // Assert
            _ = subject.Should().NotBeSameAs(value, becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void Appending(string value)
        {
            // Arrange
            SpotExternalReference subject = value;
            SpotExternalReference subjectAppended = subject + value;

            // Act
            bool result = subjectAppended == subject + value;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }
    }
}
