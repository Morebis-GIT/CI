#pragma warning disable CA1707 // Identifiers should not contain underscores

using AutoFixture;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using NUnit.Framework;

namespace ImagineCommunications.GamePlan.Domain.Tests.DomainModels.Tests
{
    [TestFixture(Category = "Domain models :: " + nameof(Pass))]

    public static class PassTests
    {
        [Test]
        [Description("Given a new Pass object when created then the DateCreated property should be set to null.")]
        public static void Given_a_new_Pass_object_when_created_then_the_DateCreated_property_should_be_set_to_null()
        {
            // Arrange
            /* Empty */

            // Act
            var labrat = new Pass();

            // Assert
            Assert.That(labrat.DateCreated, Is.Null);
        }

        [Test]
        [Description("Given a new Pass object when being hydrated then the DateModified property should not change while the model is not completely hydrated")]
        public static void Given_a_new_Pass_object_when_being_hydrated_then_the_DateModified_property_should_not_change_while_the_model_is_not_completely_hydrated()
        {
            // Arrange
            var labrat = new Pass();
            var dateModifiedBeforeHydration = labrat.DateModified;

            var fixture = new Fixture();

            // Act
            labrat.Id = fixture.Create<int>();

            // Assert
            Assert.That(labrat.DateModified, Is.EqualTo(dateModifiedBeforeHydration));
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
