using FluentAssertions;
using xggameplan.Common;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.Common.Tests.Comparers.Tests
{
    [Trait("Comparers", "AlphaNumeric")]
    public class AlphaNumericComparerTests
    {
        [Theory(DisplayName =
            "Compare two values using the same ordering as the StrCmpLogicalW function (shlwapi.h)"
            )]
        [MemberData(
            nameof(_sourceProperty),
            MemberType = typeof(AlphaNumericComparerTests)
            )]
        public void CompareTwoValues_ResultOrderMustBeSorted(
            string valueA,
            string valueB,
            int cmpResult
            )
        {
            // Arrange
            var testSubject = new AlphaNumericComparer();

            // Act
            int result = testSubject.Compare(valueA, valueB);

            // Assert
            _ = result.Should().Be(cmpResult, null);
        }

        private const int XIsEqualToY = 0;
        private const int XIsLessThanY = -1;
        private const int XIsGreaterThanY = 1;

        public static readonly object[][] _sourceProperty = new object[][] {
            new object[] { null, null, XIsEqualToY },
            new object[] { null, "", XIsLessThanY },
            new object[] { "", null, XIsGreaterThanY },
            new object[] { null, "A", XIsLessThanY },
            new object[] { "A", null, XIsGreaterThanY },
            new object[] { "", "", XIsEqualToY },

            new object[] { "A", "A", XIsEqualToY },
            new object[] { "A", "B", XIsLessThanY },
            new object[] { "B", "A", XIsGreaterThanY },

            new object[] { "AA", "AA", XIsEqualToY },
            new object[] { "AA", "AB", XIsLessThanY },
            new object[] { "AB", "AA", XIsGreaterThanY },

            new object[] { "1", "1", XIsEqualToY },
            new object[] { "1", "2", XIsLessThanY },
            new object[] { "2", "1", XIsGreaterThanY },

            new object[] { "1A", "1A", XIsEqualToY },
            new object[] { "1A", "1B", XIsLessThanY },
            new object[] { "1B", "1A", XIsGreaterThanY },

            new object[] { "1A", "2A", XIsLessThanY },
            new object[] { "2A", "1A", XIsGreaterThanY },

            new object[] { "1", "A", XIsLessThanY },
            new object[] { "A", "1", XIsGreaterThanY },

            new object[] { "1", "10", XIsLessThanY },
            new object[] { "10", "1", XIsGreaterThanY },
            new object[] { "10A", "1A", XIsGreaterThanY },
            new object[] { "10A", "", XIsGreaterThanY },

            new object[] { "2string", "3string", XIsLessThanY },
            new object[] { "3string", "20string", XIsLessThanY },
            new object[] { "20string", "st2ring", XIsLessThanY },
            new object[] { "st2ring", "st3ring", XIsLessThanY },
            new object[] { "st3ring", "st20ring", XIsLessThanY },
            new object[] { "st20ring", "string2", XIsLessThanY },
            new object[] { "string2", "string3", XIsLessThanY },
            new object[] { "string3", "string20", XIsLessThanY },

            new object[] { "October Activity 4340104323", "October Activity 4340104324", XIsLessThanY },
            new object[] { "October Activity 4340104323", "October Activity 4340104322", XIsGreaterThanY },
        };
    }
}
