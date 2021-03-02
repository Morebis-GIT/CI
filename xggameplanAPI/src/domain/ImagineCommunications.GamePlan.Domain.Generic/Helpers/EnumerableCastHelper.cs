using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ImagineCommunications.GamePlan.Domain.Generic.Helpers
{
    public static class EnumerableCastHelper
    {
        private static readonly MethodInfo CastGenericMethodInfo =
            typeof(EnumerableCastHelper).GetMethod(nameof(Cast), BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetGenericMethodDefinition();

        private static object Cast<TResult>(IEnumerable<object> data)
        {
            return data.Cast<TResult>().ToArray();
        }

        public static object CastToArray(IEnumerable<object> data, Type toType)
        {
            return CastGenericMethodInfo.MakeGenericMethod(toType).Invoke(null, new object[] {data});
        }
    }
}
