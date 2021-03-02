using FluentAssertions;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.TypeTests
{
    [Collection("Smooth types")]
    [Trait("Types", nameof(ProductExternalReference))]
    public class ProductExternalReferenceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void AssigningAValue(string value)
        {
            // Arrange

            // Act
            ProductExternalReference result = value;

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
            ProductExternalReference subject = value;

            // Act
            string result = subject;

            // Assert
            _ = value.Should().BeEquivalentTo(result, becauseArgs: null);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Hi")]
        public void ProductExternalReferenceToStringEquality(string value)
        {
            // Arrange
            ProductExternalReference subject = value;

            // Act
            bool result = subject.Equals(value);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToProductExternalReferenceEquality(string value)
        {
            // Arrange
            ProductExternalReference subject = value;

            // Act
            bool result = value.Equals(subject);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToProductExternalReferenceEqualityOperator(string value)
        {
            // Arrange
            ProductExternalReference subject = value;

            // Act
            bool result = value == subject;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void StringToProductExternalReferenceInequalityOperator(string value)
        {
            // Arrange
            ProductExternalReference subject = value;

            // Act
            bool result = subject != "Badger";

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void ProductExternalReferenceToProductExternalReferenceEquality(
            string left,
            string right
            )
        {
            // Arrange
            ProductExternalReference subjectLeft = left;
            ProductExternalReference subjectRight = right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void ProductExternalReferenceToProductExternalReferenceAsObjectEquality(
            string left,
            string right
            )
        {
            // Arrange
            ProductExternalReference subjectLeft = left;
            object subjectRight = (ProductExternalReference)right;

            // Act
            bool result = subjectLeft.Equals(subjectRight);

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Hi", "Hi")]
        public void ProductExternalReferenceToStringAsObjectEquality(
            string left,
            string right
            )
        {
            // Arrange
            ProductExternalReference subjectLeft = left;
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
            ProductExternalReference subject = value;

            // Assert
            _ = subject.Should().NotBeSameAs(value, becauseArgs: null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hi")]
        public void Appending(string value)
        {
            // Arrange
            ProductExternalReference subject = value;
            ProductExternalReference subjectAppended = subject + value;

            // Act
            bool result = subjectAppended == subject + value;

            // Assert
            _ = result.Should().BeTrue(becauseArgs: null);
        }
    }
}
