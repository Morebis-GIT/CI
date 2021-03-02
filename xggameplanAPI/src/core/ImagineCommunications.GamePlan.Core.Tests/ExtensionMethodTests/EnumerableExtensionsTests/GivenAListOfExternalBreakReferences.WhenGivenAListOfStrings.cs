using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using xggameplan.common.Extensions;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    [Trait("Extensions", "Converting references to CSV")]
    public static partial class GivenAListOfExternalBreakReferences
    {
        public static class WhenGivenAListOfStrings
        {
            [Fact]
            public static void ReturnCommaSeparatedListOfExternalBreakIdsGivenHugeList()
            {
                // Arrange
                var fixture = new Fixture();
                var someBreaks = fixture
                    .CreateMany<Break>(3000)
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
            public static void ReturnCommaSeparatedListOfExternalBreakIdsGivenSmallList()
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
            public static void ReturnCommaSeparatedListOfExternalBreakIdsGivenTinyList()
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
            public static void ReturnEmptyListWhenGivenNullList()
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
