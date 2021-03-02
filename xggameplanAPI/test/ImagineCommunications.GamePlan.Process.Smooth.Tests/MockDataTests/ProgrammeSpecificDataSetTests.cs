using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Trait("Smooth", "Loading programme specific datasets")]
    public class ProgrammeSpecificDataSetTests
    {
        private readonly Fixture _fixture;

        private static DateTimeRange ProgrammeDateTimes => (
            new DateTime(2020, 12, 25),
            new DateTime(2020, 12, 25)
            );

        public ProgrammeSpecificDataSetTests() => _fixture = new Fixture();

        [Fact(DisplayName = "Trying to load the wrong model type throws an exception")]
        public void TryingToLoadTheWrongModelTypeThrows()
        {
            // Assert
            var result = Assert.Throws<InvalidOperationException>(() =>
            {
                // Arrange
                var source = _fixture
                    .CreateMany<Programme>(1)
                    .ToList();

                // Act
                _ = ProgrammeSpecificDataSet.GetForPeriod<Programme>(
                    ProgrammeDateTimes,
                    source
                    );
            });

            _ = result.Message.Should().Be("Only breaks and spots are currently valid.", because: null);
        }

        [Fact(DisplayName = "Get breaks for a programme")]
        public void GetBreaksForTheProgrammes()
        {
            // Arrange
            var source = _fixture
                .Build<Break>()
                .With(p => p.ScheduledDate, new DateTime(2020, 12, 25))
                .CreateMany(3)
                .ToList();

            // This break is for a different programme and should not be returned.
            source[1].ScheduledDate = new DateTime(2020, 12, 27);

            // Act
            IReadOnlyCollection<Break> result = ProgrammeSpecificDataSet.GetForPeriod<Break>(
                ProgrammeDateTimes,
                source
                );

            // Assert
            _ = result.Should().HaveCount(2, becauseArgs: null);
        }

        [Fact(DisplayName = "Get spots for a programme")]
        public void GetSpotsForTheProgrammes()
        {
            // Arrange
            var source = _fixture
                .Build<Spot>()
                .With(p => p.StartDateTime, new DateTime(2020, 12, 25))
                .With(p => p.EndDateTime, new DateTime(2020, 12, 25))
                .CreateMany(3)
                .ToList();

            // Spots for a different programme; should not be returned.
            source[1].StartDateTime = new DateTime(2020, 12, 29);
            source[1].EndDateTime = new DateTime(2020, 12, 29);

            // Act
            IReadOnlyCollection<Spot> result = ProgrammeSpecificDataSet.GetForPeriod<Spot>(
                ProgrammeDateTimes,
                source
                );

            // Assert
            _ = result.Should().HaveCount(2, becauseArgs: null);
        }
    }
}
