using System.Collections.Generic;
using FluentAssertions;
using xggameplan.Extensions;
using Xunit;

namespace ImagineCommunications.GamePlan.Domain.XUnit.Tests.Common.Tests.Extensions.Tests
{
    [Trait("Extensions", "Sorting")]
    public class GenericSortTest
    {
        [Theory(DisplayName = "Generic sort by single field using a default comparer equals to order by")]
        [MemberData(
            nameof(GenericSortTestData.SortByDefaultComparerData),
            MemberType = typeof(GenericSortTestData)
            )]
        public void GenericSortBySingleField_DefaultComparer_EqualsToOrderBy(
            string property,
            string orderDirection,
            IEnumerable<GenericSortTestData.TestSortable> source,
            IEnumerable<GenericSortTestData.TestSortable> target
            )
        {
            var sorted = source.Sort(property, orderDirection);
            _ = sorted.Should().BeEquivalentTo(target);
        }

        [Theory(DisplayName = "Generic sort by single field using a custom comparer equals to order by")]
        [MemberData(
            nameof(GenericSortTestData.SortByCustomComparerData),
            MemberType = typeof(GenericSortTestData)
            )]
        public void GenericSortBySingleField_CustomComparer_EqualsToOrderBy(
            string property,
            string orderDirection,
            IComparer<object> comparer,
            IEnumerable<GenericSortTestData.TestSortable> source,
            IEnumerable<GenericSortTestData.TestSortable> target
            )
        {
            var sorted = source.Sort(property, orderDirection, comparer);
            _ = sorted.Should().BeEquivalentTo(target);
        }

        [Theory(DisplayName = "Generic sort by multiple fields using a default comparer equals to order by")]
        [MemberData(
            nameof(GenericSortTestData.SortMultipleFieldsByDefaultComparerData),
            MemberType = typeof(GenericSortTestData)
            )]
        public void GenericSortByMultipleField_DefaultComparer_EqualsToOrderBy(
            List<(string, string, IComparer<object>)> props,
            IEnumerable<GenericSortTestData.TestSortable> source,
            IEnumerable<GenericSortTestData.TestSortable> target
            )
        {
            var sorted = source.MultipleSort(props);
            _ = sorted.Should().BeEquivalentTo(target);
        }

        [Theory(DisplayName = "Generic sort by multiple fields using a custom comparer equals to order by")]
        [MemberData(
            nameof(GenericSortTestData.SortMultipleFieldsByCustomComparerData),
            MemberType = typeof(GenericSortTestData)
            )]
        public void GenericSortByMultipleField_CustomComparer_EqualsToOrderBy(
            List<(string, string, IComparer<object>)> props,
            IEnumerable<GenericSortTestData.TestSortable> source,
            IEnumerable<GenericSortTestData.TestSortable> target
            )
        {
            var sorted = source.MultipleSort(props);
            _ = sorted.Should().BeEquivalentTo(target);
        }
    }
}
