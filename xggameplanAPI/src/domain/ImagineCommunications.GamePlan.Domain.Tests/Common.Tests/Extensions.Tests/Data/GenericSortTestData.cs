using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Tests.Common.Tests.Extensions.Tests.Data
{
    public static class GenericSortTestData
    {
        public class TestSortable
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public TestType? Type { get; set; }
        }

        public enum TestType
        {
            C = 1,
            A = 2,
            B = 3,
            F = 4,
            D = 5,
            E = 6
        }

        public class TestViceVersaIntComparer : IComparer<object>
        {
            public int Compare(object x, object y)
            {
                if (x == null)
                {
                    return y == null ? 0 : 1;
                }
                else
                {
                    if (y == null)
                    {
                        return -1;
                    }
                    else
                    {
                        var xNum = (int)x;
                        var yNum = (int)y;
                        return -1 * xNum.CompareTo(yNum);
                    }
                }
            }
        }

        public class TestEnumNameComparer : IComparer<object>
        {
            public int Compare(object x, object y)
            {
                if (x == null)
                {
                    return y == null ? 0 : -1;
                }
                else
                {
                    if (y == null)
                    {
                        return 1;
                    }
                    else
                    {
                        var xStr = ((Enum)x).ToString();
                        var yStr = ((Enum)y).ToString();
                        return string.Compare(xStr, yStr, StringComparison.InvariantCulture);
                    }
                }
            }
        }

        private static readonly List<TestSortable> SortableList = new List<TestSortable>()
        {
            new TestSortable(){ Id = 1, Name = "A", Type = TestType.D },
            new TestSortable(){ Id = 2, Name = "B", Type = TestType.A },
            new TestSortable(){ Id = 5, Name = "C", Type = TestType.B },
            new TestSortable(){ Id = 3, Name = "D", Type = null },
            new TestSortable(){ Id = 4, Name = "E", Type = TestType.F },
            new TestSortable(){ Id = 6, Name = "F", Type = TestType.E },
            new TestSortable(){ Id = null, Name = "C", Type = TestType.D },
            new TestSortable(){ Id = 3, Name = "F", Type = null },
            new TestSortable(){ Id = 1, Name = "B", Type = TestType.B },
            new TestSortable(){ Id = 2, Name = "C", Type = TestType.E }
        };

        public static readonly object[] SortByDefaultComparerData = {
            new object[]
            {
                 "Id","asc", SortableList, SortableList.OrderBy(s=> s.Id)
            },
            new object[]
            {
                  "Id","desc", SortableList, SortableList.OrderByDescending(s=> s.Id)
            },
            new object[]
            {
                 "Name","asc", SortableList, SortableList.OrderBy(s=> s.Name)
            },
            new object[]
            {
                  "Name","desc", SortableList, SortableList.OrderByDescending(s=> s.Name)
            },
            new object[]
            {
                 "Type","asc", SortableList, SortableList.OrderBy(s=> s.Type)
            },
            new object[]
            {
                  "Type","desc", SortableList, SortableList.OrderByDescending(s=> s.Type)
            }
        };

        public static readonly object[] SortByCustomComparerData = {
            new object[]
            {
                 "Id","asc", new TestViceVersaIntComparer(), SortableList, SortableList.OrderByDescending(s=> s.Id)
            },
            new object[]
            {
                  "Id","desc", new TestViceVersaIntComparer(), SortableList, SortableList.OrderBy(s=> s.Id)
            },
            new object[]
            {
                 "Name","asc", null, SortableList, SortableList.OrderBy(s=> s.Name)
            },
            new object[]
            {
                  "Name","desc", null, SortableList, SortableList.OrderByDescending(s=> s.Name)
            },
             new object[]
            {
                 "Type","asc", new TestEnumNameComparer(), SortableList, SortableList.OrderBy(s=> s.Type.ToString())
            },
            new object[]
            {
                  "Type","desc", new TestEnumNameComparer(), SortableList, SortableList.OrderByDescending(s=> s.Type.ToString())
            }
        };

        public static readonly object[] SortMultipleFieldsByDefaultComparerData = {
            new object[]
            {
                 new List<(string,string, IComparer<object>)>(){
                     ("Id", "asc", null),
                     ("Name", "asc", null)
                 },
                SortableList, SortableList.OrderBy(s=> s.Id).ThenBy(s=> s.Name)
            },
            new object[]
            {
                  new List<(string,string, IComparer<object>)>(){
                      ("Id", "desc", null),
                      ("Type", "asc", null)
                  },
                SortableList, SortableList.OrderByDescending(s=> s.Id).ThenBy(s=> s.Type)
            },
            new object[]
            {
                 new List<(string,string, IComparer<object>)>(){
                     ("Name", "asc", null),
                     ("Id", "asc", null)
                 }, SortableList, SortableList.OrderBy(s=> s.Name).ThenBy(s=> s.Id)
            },
            new object[]
            {
                  new List<(string,string, IComparer<object>)>(){
                      ("Name", "desc", null),
                      ("Id", "asc", null)
                  }, SortableList, SortableList.OrderByDescending(s=> s.Name).ThenBy(s=> s.Id)
            },
            new object[]
            {
                 new List<(string,string, IComparer<object>)>(){
                     ("Type", "asc", null),
                     ("Name", "desc", null)
                 }, SortableList, SortableList.OrderBy(s=> s.Type).ThenByDescending(s=> s.Name)
            },
            new object[]
            {
                  new List<(string,string, IComparer<object>)>(){
                      ("Type", "desc", null),
                      ("Id", "asc", null)
                  }, SortableList, SortableList.OrderByDescending(s=> s.Type).ThenBy(s=> s.Id)
            }
        };

        public static readonly object[] SortMultipleFieldsByCustomComparerData = {
            new object[]
            {
                 new List<(string,string, IComparer<object>)>(){
                     ("Id", "asc", null),
                     ("Name", "asc", null)
                 },
                SortableList, SortableList.OrderBy(s=> s.Id).ThenBy(s=> s.Name)
            },
            new object[]
            {
                  new List<(string,string, IComparer<object>)>(){
                      ("Id", "desc", null),
                      ("Type", "asc", new TestEnumNameComparer())
                  },
                SortableList, SortableList.OrderByDescending(s=> s.Id).ThenBy(s=> s.Type.ToString())
            },
            new object[]
            {
                 new List<(string,string, IComparer<object>)>(){
                     ("Name", "asc", null),
                     ("Id", "asc", null)
                 }, SortableList, SortableList.OrderBy(s=> s.Name).ThenBy(s=> s.Id)
            },
            new object[]
            {
                  new List<(string,string, IComparer<object>)>(){
                      ("Name", "desc", null),
                      ("Id", "asc", new TestViceVersaIntComparer())
                  }, SortableList, SortableList.OrderByDescending(s=> s.Name).ThenByDescending(s=> s.Id)
            },
            new object[]
            {
                 new List<(string,string, IComparer<object>)>(){
                     ("Type", "asc", new TestEnumNameComparer()),
                     ("Name", "desc", null)
                 }, SortableList, SortableList.OrderBy(s=> s.Type.ToString()).ThenByDescending(s=> s.Name)
            },
            new object[]
            {
                  new List<(string,string, IComparer<object>)>(){
                      ("Type", "desc", new TestEnumNameComparer()),
                      ("Id", "asc", new TestViceVersaIntComparer())
                  }, SortableList, SortableList.OrderByDescending(s=> s.Type.ToString()).ThenByDescending(s=> s.Id)
            }
        };
    }
}
