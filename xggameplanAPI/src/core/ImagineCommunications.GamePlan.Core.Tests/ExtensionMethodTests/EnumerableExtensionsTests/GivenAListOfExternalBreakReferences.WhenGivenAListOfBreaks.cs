using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using NodaTime;
using xggameplan.common.Extensions;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    [Trait("Extensions", "Converting references to CSV")]
    public static partial class GivenAListOfExternalBreakReferences
    {
        public static class WhenGivenAListOfBreaks
        {
            [Fact]
            public static void ReturnCommaSeparatedListOfExternalBreakIdsWhenGivenHugeList()
            {
                // Arrange
                var fixture = new Fixture();

                // Note: the test is *only* using the external break ref property.
                // Setting the following properties explicitly stops AutoFixture
                // building time-expensive objects that just aren't used. As
                // such, the time taken to create 3,000 Break objects in my test
                // platform reduced from approximately 3.3 seconds to 0.8 seconds.
                var someBreaks = fixture.Build<Break>()
                    .Without(p => p.ExternalBreakRef)
                    .Without(p => p.SpotReferencesList)
                    .Without(p => p.ScheduledDate)
                    .Without(p => p.BreakEfficiencyList)
                    .With(p => p.Avail, Duration.Zero)
                    .With(p => p.OptimizerAvail, Duration.Zero)
                    .CreateMany(3000)
                    .ToList();

                var expected = new StringBuilder();

                for (int idx = 0; idx < someBreaks.Count; ++idx)
                {
                    var value = $"ztas{idx}";
                    _ = expected.Append(value).Append(", ");

                    someBreaks[idx].ExternalBreakRef = value;
                }

                _ = expected.Remove(expected.Length - 2, 2);

                // Act
                string result = someBreaks.ReducePropertyToCsv(x => x.ExternalBreakRef);

                // Assert
                _ = result.Should().BeEquivalentTo(expected.ToString());
            }

            [Fact]
            public static void ReturnCommaSeparatedListOfExternalBreakIdsWhenGivenSmallList()
            {
                // Arrange
                var fixture = new Fixture();
                var someBreaks = fixture
                    .CreateMany<Break>(3)
                    .ToList();

                for (int idx = 0; idx < someBreaks.Count; ++idx)
                {
                    someBreaks[idx].ExternalBreakRef = $"ztas{idx}";
                }

                var expected = new StringBuilder();
                for (int idx = 0; idx < someBreaks.Count; idx++)
                {
                    _ = expected.Append("ztas").Append(idx).Append(", ");
                }

                _ = expected.Remove(expected.Length - 2, 2);

                // Act
                string result = someBreaks.ReducePropertyToCsv(x => x.ExternalBreakRef);

                // Assert
                _ = result.Should().BeEquivalentTo(expected.ToString());
            }

            [Fact]
            public static void ReturnCommaSeparatedListOfExternalBreakIdsWhenGivenTinyList()
            {
                // Arrange
                var fixture = new Fixture();
                var someBreaks = fixture
                    .CreateMany<Break>(1)
                    .ToList();

                for (int idx = 0; idx < someBreaks.Count; ++idx)
                {
                    someBreaks[idx].ExternalBreakRef = $"ztas{idx}";
                }

                var expected = new StringBuilder();
                for (int idx = 0; idx < someBreaks.Count; idx++)
                {
                    _ = expected.Append("ztas").Append(idx).Append(", ");
                }

                _ = expected.Remove(expected.Length - 2, 2);

                // Act
                string result = someBreaks.ReducePropertyToCsv(x => x.ExternalBreakRef);

                // Assert
                _ = result.Should().BeEquivalentTo(expected.ToString());
            }

            [Fact]
            public static void ReturnEmptyStringWhenGivenEmptyList()
            {
                // Arrange
                /* Empty */

                // Act
                string result = Array.Empty<Break>().ReducePropertyToCsv(x => x.ExternalBreakRef);

                // Assert
                _ = result.Should().BeEmpty();
            }

            [Fact]
            public static void ReturnEmptyListWhenGivenNull()
            {
                // Arrange
                /* Empty */

                // Act
                string result = ((List<Break>)null).ReducePropertyToCsv(x => x.ExternalBreakRef);

                // Assert
                _ = result.Should().BeEmpty();
            }
        }
    }
}
