using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using xggameplan.common.Extensions;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Extensions
{
    public static class TableExtensions
    {
        private static readonly MethodInfo CompareToInstanceMethodInfo =
            ReflectionExtensions.GetMethodInfo<Table>(t =>
                t.CompareToInstance<object>(null));

        public static IRepositoryMethodParameters ToRepositoryMethodParameters(this Table table, string keyColumn, string valueColumn)
        {
            if (!table.ContainsColumn(keyColumn))
            {
                throw new ArgumentException($"'{keyColumn}' column name doesn't exist.", nameof(keyColumn));
            }

            if (!table.ContainsColumn(valueColumn))
            {
                throw new ArgumentException($"'{valueColumn}' column name doesn't exist.", nameof(valueColumn));
            }

            var dictionary = new RepositoryMethodParameters();
            foreach (var row in table.Rows)
            {
                var key = row[keyColumn];
                var value = row[valueColumn];
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }

            return dictionary;
        }

        public static IRepositoryMethodParameters ToRepositoryMethodParameters(this Table table)
        {
            if (table == null)
            {
                return null;
            }

            if (table.Header.Count < 2)
            {
                throw new Exception("Table can't be converted to dictionary.");
            }

            return ToRepositoryMethodParameters(table, table.Header.First(), table.Header.Skip(1).First());
        }

        public static IDictionary<string, string> ToDictionary(this Table table)
        {
            return ToRepositoryMethodParameters(table);
        }

        public static void CompareToInstance(this Table table, object instance, Type instanceType)
        {
            _ = CompareToInstanceMethodInfo.MakeGenericMethod(instanceType).Invoke(null, new object[] { table, instance });
        }
    }
}
