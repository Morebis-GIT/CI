using System;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Moq;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.ModelTests
{
    [Trait("Smooth", nameof(SmoothFailureMessagesForSpotsCollection))]
    public class SmoothFailureMessagesForSpotsCollectionTests
    {
        [Fact(DisplayName =
            "Given a new uninitialised collection " +
            "When adding a single item without a restriction " +
            "Then iterating the collection returns the same single item")]
        public void SingleItemInSingleItemOutNoRestriction()
        {
            // Arrange
            var target = new SmoothFailureMessagesForSpotsCollection();
            var spotUid = Guid.NewGuid();
            const SmoothFailureMessages specimen = SmoothFailureMessages.T1_BreakPosition;

            // Act
            target.Add(spotUid, specimen);

            // Assert
            _ = target.Should().ContainSingle(becauseArgs: null);
            _ = target[spotUid].Failures[0].FailureMessage.Should().Be(specimen, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a new uninitialised collection " +
            "When adding a single item including a restriction " +
            "Then iterating the collection returns the same single item")]
        public void SingleItemInSingleItemOutIncludingRestriction()
        {
            // Arrange
            var target = new SmoothFailureMessagesForSpotsCollection();
            var spotUid = Guid.NewGuid();
            var restriction = new Mock<Restriction>().Object;

            const SmoothFailureMessages specimen = SmoothFailureMessages.T1_BreakPosition;

            // Act
            target.Add(spotUid, specimen, restriction);

            // Assert
            _ = target.Should().ContainSingle(becauseArgs: null);
            _ = target[spotUid].Failures[0].FailureMessage.Should().Be(specimen, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a new collection " +
            "And the collection is initialised " +
            "When adding a single item without a restriction " +
            "Then iterating the collection returns the same single item")]
        public void SingleItemInSingleItemOutNoRestrictionInitialisedCollection()
        {
            // Arrange
            var target = new SmoothFailureMessagesForSpotsCollection();
            var spotUid = Guid.NewGuid();
            const SmoothFailureMessages specimen = SmoothFailureMessages.T1_BreakPosition;

            // Act
            target.InitialiseForSpot(spotUid);
            target.Add(spotUid, specimen);

            // Assert
            _ = target.Should().ContainSingle(becauseArgs: null);
            _ = target[spotUid].Failures[0].FailureMessage.Should().Be(specimen, becauseArgs: null);
        }

        [Fact(DisplayName =
            "Given a new collection " +
            "And the collection is initialised " +
            "When adding a single item including a restriction " +
            "Then iterating the collection returns the same single item")]
        public void SingleItemInSingleItemOutIncludingRestrictionInitialisedCollection()
        {
            // Arrange
            var target = new SmoothFailureMessagesForSpotsCollection();
            var spotUid = Guid.NewGuid();
            var restriction = new Mock<Restriction>().Object;

            const SmoothFailureMessages specimen = SmoothFailureMessages.T1_BreakPosition;

            // Act
            target.InitialiseForSpot(spotUid);
            target.Add(spotUid, specimen, restriction);

            // Assert
            _ = target.Should().ContainSingle(becauseArgs: null);
            _ = target[spotUid].Failures[0].FailureMessage.Should().Be(specimen, becauseArgs: null);
        }
    }
}
