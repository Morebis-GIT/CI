using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Extensions.Tests.Data;
using NUnit.Framework;
using xggameplan.Extensions;
using static ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Extensions.Tests.Data.GenericSortTestData;

namespace ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Extensions.Tests
{
    [TestFixture(Category = "Extensions :: Sorting")]
    public class GenericSortTest
    {
        [TestCaseSource(typeof(GenericSortTestData), nameof(SortByDefaultComparerData))]
        [Description("Generic sort by single field using a default comparer equals to order by")]
        public void GenericSortBySingleField_DefaultComparer_EqualsToOrderBy(
            string property,
            string orderDirection,
            IEnumerable<TestSortable> source,
            IEnumerable<TestSortable> target
            )
        {
            var sorted = source.Sort(property, orderDirection);
            Assert.IsTrue(sorted.SequenceEqual(target));
        }

        [TestCaseSource(typeof(GenericSortTestData), nameof(SortByCustomComparerData))]
        [Description("Generic sort by single field using a custom comparer equals to order by")]
        public void GenericSortBySingleField_CustomComparer_EqualsToOrderBy(
            string property,
            string orderDirection,
            IComparer<object> comparer,
            IEnumerable<TestSortable> source,
            IEnumerable<TestSortable> target
            )
        {
            var sorted = source.Sort(property, orderDirection, comparer);
            Assert.IsTrue(sorted.SequenceEqual(target));
        }

        [TestCaseSource(typeof(GenericSortTestData), nameof(SortMultipleFieldsByDefaultComparerData))]
        [Description("Generic sort by multiple fields using a default comparer equals to order by")]
        public void GenericSortByMultipleField_DefaultComparer_EqualsToOrderBy(
            List<(string, string, IComparer<object>)> props,
            IEnumerable<TestSortable> source,
            IEnumerable<TestSortable> target
            )
        {
            var sorted = source.MultipleSort(props);
            Assert.IsTrue(sorted.SequenceEqual(target));
        }

        [TestCaseSource(typeof(GenericSortTestData), nameof(SortMultipleFieldsByCustomComparerData))]
        [Description("Generic sort by multiple fields using a custom comparer equals to order by")]
        public void GenericSortByMultipleField_CustomComparer_EqualsToOrderBy(
            List<(string, string, IComparer<object>)> props,
            IEnumerable<TestSortable> source,
            IEnumerable<TestSortable> target
            )
        {
            var sorted = source.MultipleSort(props);
            Assert.IsTrue(sorted.SequenceEqual(target));
        }
    }
}
